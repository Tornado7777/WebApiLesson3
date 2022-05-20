using Dapper;
using MetricsAgent.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.Extensions.Options;
using MetricsAgent.Controllers;

namespace MetricsAgent.Services.Impl
{

    // TODO: Домашняя работа
    // 1. Перепишите все репозитории в приложении агента метрик,
    //    используя Dapper вместо сырого чтения данных.


    public class DotNetMetricsRepository : IDotNetMetricsRepository
    {
        private readonly IOptions<DatabaseOptions> _databaseOptions;

        public DotNetMetricsRepository(IOptions<DatabaseOptions> databaseOptions)
        {
            _databaseOptions = databaseOptions;
        }

        public void Create(DotNetMetric item)
        {
           
            DatabaseOptions databaseOptions = _databaseOptions.Value;
            using var connection = new SQLiteConnection(databaseOptions.ConnectionString);
            // Запрос на добавление данных с плейсхолдерами для параметров
            connection.Execute("INSERT INTO dotNetmetrics(value, time) VALUES(@value, @time)",
            // Анонимный объект с параметрами запроса
            new
            {
                // Value подставится на место "@value" в строке запроса
                // Значение запишется из поля Value объекта item
                value = item.Value,
                // Записываем в поле time количество секунд
                time = item.Time
            });
        }
        public void Delete(int id)
        {
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            connection.Execute("DELETE FROM dotNetmetrics WHERE id=@id", new { id = id });
        }
        public void Update(DotNetMetric item)
        {
            
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            connection.Execute("UPDATE dotNetmetrics SET value = @value, time = @time WHERE id = @id",
            new
            {
                value = item.Value,
                time = item.Time,
                id = item.Id
            });
        }
        public IList<DotNetMetric> GetAll()
        {
            
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            List<DotNetMetric> metrics = connection.Query<DotNetMetric>("SELECT * FROM dotNetmetrics").ToList();
            return metrics;
        }
        public DotNetMetric GetById(int id)
        {
            
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            DotNetMetric metric = connection.QuerySingle<DotNetMetric>("SELECT Id, Time, Value FROM dotNetmetrics WHERE id = @id",
            new { id = id });
            return metric;
        }

        /// <summary>
        /// Получение данных по нагрузке на ЦП за период
        /// </summary>
        /// <param name="timeFrom">Время начала периода</param>
        /// <param name="timeTo">Время окончания периода</param>
        /// <returns></returns>
        public IList<DotNetMetric> GetByTimePeriod(TimeSpan timeFrom, TimeSpan timeTo)
        {
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            string table = "dotNetmetrics";
            List<DotNetMetric> metrics = connection.Query<DotNetMetric>($"SELECT * FROM {table} where time >= @timeFrom and time <= @timeTo",
                new { timeFrom = timeFrom.TotalSeconds, timeTo = timeTo.TotalSeconds }).ToList();
            return metrics;
        }
    }
}
