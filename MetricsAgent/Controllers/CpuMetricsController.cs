using AutoMapper;
using MetricsAgent.Models;
using MetricsAgent.Models.Requests;
using MetricsAgent.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/cpu")]
    [ApiController]
    public class CpuMetricsController : ControllerBase
    {

        #region Пример взаимодействия с СУБД, потом удалим

        [HttpGet("sql-test")]
        public IActionResult TryToSqlLite()
        {
            string cs = "Data Source=:memory:";
            string stm = "SELECT SQLITE_VERSION()";
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using var cmd = new SQLiteCommand(stm, con);
                string version = cmd.ExecuteScalar().ToString();
                return Ok(version);
            }
        }

        [HttpGet("sql-read-write-test")]
        public IActionResult TryToInsertAndRead()
        {
            // Создаём строку подключения в виде базы данных в оперативной памяти
            string connectionString = "Data Source=:memory:";
            // Создаём соединение с базой данных
            using (var connection = new SQLiteConnection(connectionString))
            {
                // Открываем соединение
                connection.Open();
                // Создаём объект, через который будут выполняться команды к базе данных
                using (var command = new SQLiteCommand(connection))
                {
                    // Задаём новый текст команды для выполнения
                    // Удаляем таблицу с метриками, если она есть в базе данных
                    command.CommandText = "DROP TABLE IF EXISTS cpumetrics";
                    // Отправляем запрос в базу данных
                    command.ExecuteNonQuery();
                    // Создаём таблицу с метриками
                    command.CommandText =
                        @"CREATE TABLE cpumetrics(id INTEGER
                        PRIMARY KEY,
                        value INT, time INT)";
                    command.ExecuteNonQuery();
                    // Создаём запрос на вставку данных
                    command.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(10, 1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(50, 2)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(75, 4)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(90, 5)";
                    command.ExecuteNonQuery();
                    // Создаём строку для выборки данных из базы
                    // LIMIT 3 обозначает, что мы достанем только 3 записи
                    string readQuery = "SELECT * FROM cpumetrics LIMIT 3";
                    // Создаём массив, в который запишем объекты с данными из базы данных
                    var returnArray = new CpuMetric[3];
                    // Изменяем текст команды на наш запрос чтения
                    command.CommandText = readQuery;
                    // Создаём читалку из базы данных
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Счётчик, чтобы записать объект в правильное место в массиве
                        var counter = 0;
                        // Цикл будет выполняться до тех пор, пока есть что читать из базы данных
                        while (reader.Read())
                        {

                            // Создаём объект и записываем его в массив
                            returnArray[counter] = new CpuMetric
                            {
                                Id = reader.GetInt32(0), // Читаем данные, полученные из базы данных
                                Value = reader.GetInt32(1), // преобразуя к целочисленному типу
                                Time = reader.GetInt64(2)
                            };
                            // Увеличиваем значение счётчика
                            counter++;
                        }
                    }
                    // Оборачиваем массив с данными в объект ответа и возвращаем пользователю
                    return Ok(returnArray);
                }
            }
        }

        #endregion

        #region Services

        private readonly ICpuMetricsRepository _cpuMetricsRepository;
        private readonly ILogger<CpuMetricsController> _logger;
        private readonly IMapper _mapper;
        //private readonly SampleData _sampleData;
        //private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Constructors

        public CpuMetricsController(
            //IServiceProvider serviceProvider,
            //SampleData sampleData,
            IMapper mapper,
            ILogger<CpuMetricsController> logger,
            ICpuMetricsRepository cpuMetricsRepository)
        {
            //_sampleData = sampleData;
            //_serviceProvider = serviceProvider;
            _mapper = mapper;
            _logger = logger;
            _cpuMetricsRepository = cpuMetricsRepository;
        }

        #endregion

        [HttpPost("create")]
        public IActionResult Create([FromBody] CpuMetricCreateRequest request)
        {
            CpuMetric cpuMetric = new CpuMetric
            {
                Time = request.Time.TotalSeconds,
                Value = request.Value
            };

            _cpuMetricsRepository.Create(cpuMetric);

            if (_logger != null)
                _logger.LogDebug("Успешно добавили новую cpu метрику: {0}", cpuMetric);

            return Ok();
        }

        // TODO: Домашняя работа
        // 2. Настройте AutoMapper для остальных объектов в приложениях: преобразование из DTO в
        // модели базы и обратно.

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var metrics = _cpuMetricsRepository.GetAll();
            var response = new AllCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            foreach (var metric in metrics)
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));

            return Ok(response);
        }

        [HttpGet("all-old-v2")]
        public IActionResult GetAllOldV2()
        {
            MapperConfiguration config = new MapperConfiguration( 
                cfg => cfg.CreateMap<CpuMetric, CpuMetricDto>().
                ForMember(x => x.Time, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.Time))));

            IMapper mapper = config.CreateMapper();

            var metrics = _cpuMetricsRepository.GetAll();
            var response = new AllCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(mapper.Map<CpuMetricDto>(metric));
                //response.Metrics.Add(new CpuMetricDto
                //{
                //    Time = TimeSpan.FromSeconds(metric.Time),
                //    Value = metric.Value,
                //    Id = metric.Id
                //});
            }


            return Ok(response);
        }

        [HttpGet("all-old-v1")]
        public IActionResult GetAllOldV1()
        {
            var metrics = _cpuMetricsRepository.GetAll();
            var response = new AllCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (var metric in metrics)
            {
                response.Metrics.Add(new CpuMetricDto
                {
                    Time = TimeSpan.FromSeconds(metric.Time),
                    Value = metric.Value,
                    Id = metric.Id
                });
            }
            return Ok(response);
        }

        /// <summary>
        /// Получить статистику по нагрузке на ЦП за период с учетом перцентиля
        /// </summary>
        /// <param name="fromTime">Время начала периода</param>
        /// <param name="toTime">Время окончания периода</param>
        /// <param name="percentile">Перцентиль</param>
        /// <returns></returns>
        [HttpGet("from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetCpuMetricsByPercentile(
            [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime, [FromRoute] float percentile)
        {
            return Ok();
        }

        /// <summary>
        /// Получить статистику по нагрузке на ЦП за период
        /// </summary>
        /// <param name="fromTime">Время начала периода</param>
        /// <param name="toTime">Время окончания периода</param>
        /// <returns></returns>
        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetCpuMetrics(
            [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            var metrics = _cpuMetricsRepository.GetByTimePeriod(fromTime, toTime);
            var response = new AllCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };
            foreach (var metric in metrics)
            {
                response.Metrics.Add(new CpuMetricDto
                {
                    Time = TimeSpan.FromSeconds(metric.Time),
                    Value = metric.Value,
                    Id = metric.Id
                });
            }
            return Ok(response);
        }

    }
}
