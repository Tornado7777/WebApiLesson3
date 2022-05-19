using MetricsAgent.Models;
using MetricsAgent.Models.Requests;
using Microsoft.AspNetCore.Http;
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

        private ICpuMetricsRepository _cpuMetricsRepository;
        private ILogger<CpuMetricsController> _logger;


        public CpuMetricsController(
            ILogger<CpuMetricsController> logger,
            ICpuMetricsRepository cpuMetricsRepository)
        {
            _cpuMetricsRepository = cpuMetricsRepository;
            _logger = logger;
        }


        [HttpPost("create")]
        public IActionResult Create([FromBody] CpuMetricCreateRequest request)
        {
            CpuMetric cpuMetric = new CpuMetric
            {
                Time = request.Time,
                Value = request.Value
            };

            _cpuMetricsRepository.Create(cpuMetric);

            // ДОМАШНЕЕ ЗАДАНИЕ
            // TODO: 1. Добавьте логирование всех параметров в каждый контроллер в обоих проектах.

            if (_logger != null)
                _logger.LogDebug("Успешно добавили новую cpu метрику: {0}", cpuMetric);

            return Ok();
        }

        [HttpGet("all")]
        public IActionResult GetAll()
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
                    Time = metric.Time,
                    Value = metric.Value,
                    Id = metric.Id
                });
            }
            return Ok(response);
        }


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




        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAllCluster(
            [FromRoute] TimeSpan fromTime, [FromRoute] TimeSpan toTime)
        {
            return Ok();
        }

        // TODO: Домашнее задание [Урок 2, пункт 2]
        // В проект агента сбора метрик добавьте контроллеры для сбора метрик, аналогичные
        // менеджеру сбора метрик.Добавьте методы для получения метрик с агента, доступные по
        //следующим путям
        // a. api/metrics/cpu/from/{fromTime}/to/{toTime} [ВЫПОЛНИЛИ ВМЕСТЕ]
        // b. api / metrics / dotnet / errors - count / from /{ fromTime}/ to /{ toTime}
        // c. api / metrics / network / from /{ fromTime}/ to /{ toTime}
        // d. api / metrics / hdd / left / from /{ fromTime}/ to /{ toTime}
        // e. api / metrics / ram / available / from /{ fromTime}/ to /{ toTime}
    }
}
