﻿using AutoMapper;
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
    [Route("api/metrics/hdd")]
    [ApiController]
    public class HddMetricsController : ControllerBase
    {

        #region Пример взаимодействия с СУБД

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
                    command.CommandText = "DROP TABLE IF EXISTS hddmetrics";
                    // Отправляем запрос в базу данных
                    command.ExecuteNonQuery();
                    // Создаём таблицу с метриками
                    command.CommandText =
                        @"CREATE TABLE hddmetrics(id INTEGER
                        PRIMARY KEY,
                        value INT, time INT)";
                    command.ExecuteNonQuery();
                    // Создаём запрос на вставку данных
                    command.CommandText = "INSERT INTO hddmetrics(value, time) VALUES(10, 1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO hddmetrics(value, time) VALUES(50, 2)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO hddmetrics(value, time) VALUES(75, 4)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO hddmetrics(value, time) VALUES(90, 5)";
                    command.ExecuteNonQuery();
                    // Создаём строку для выборки данных из базы
                    // LIMIT 3 обозначает, что мы достанем только 3 записи
                    string readQuery = "SELECT * FROM hddmetrics LIMIT 3";
                    // Создаём массив, в который запишем объекты с данными из базы данных
                    var returnArray = new HddMetric[3];
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
                            returnArray[counter] = new HddMetric
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

        private readonly IHddMetricsRepository _hddMetricsRepository;
        private readonly ILogger<HddMetricsController> _logger;
        private readonly IMapper _mapper;
        //private readonly SampleData _sampleData;
        //private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Constructors

        public HddMetricsController(
            //IServiceProvider serviceProvider,
            //SampleData sampleData,
            IMapper mapper,
            ILogger<HddMetricsController> logger,
            IHddMetricsRepository hddMetricsRepository)
        {
            //_sampleData = sampleData;
            //_serviceProvider = serviceProvider;
            _mapper = mapper;
            _logger = logger;
            _hddMetricsRepository = hddMetricsRepository;
        }

        #endregion

        [HttpPost("create")]
        public IActionResult Create([FromBody] HddMetricCreateRequest request)
        {
            HddMetric hddMetric = new HddMetric
            {
                Time = request.Time.TotalSeconds,
                Value = request.Value
            };

            _hddMetricsRepository.Create(hddMetric);

            if (_logger != null)
                _logger.LogDebug("Успешно добавили новую hdd метрику: {0}", hddMetric);

            return Ok();
        }

        // TODO: Домашняя работа
        // 2. Настройте AutoMapper для остальных объектов в приложениях: преобразование из DTO в
        // модели базы и обратно.

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var metrics = _hddMetricsRepository.GetAll();
            var response = new AllHddMetricsResponse()
            {
                Metrics = new List<HddMetricDto>()
            };

            foreach (var metric in metrics)
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));

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
        public IActionResult GetHddMetricsByPercentile(
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
        public IActionResult GetHddMetrics(
            [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            var metrics = _hddMetricsRepository.GetByTimePeriod(fromTime, toTime);
            var response = new AllHddMetricsResponse()
            {
                Metrics = new List<HddMetricDto>()
            };
            foreach (var metric in metrics)
            {
                response.Metrics.Add(new HddMetricDto
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
