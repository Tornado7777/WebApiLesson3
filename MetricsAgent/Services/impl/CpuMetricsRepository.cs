using Dapper;
using MetricsAgent.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Microsoft.Extensions.Options;

namespace MetricsAgent.Services.Impl
{

    // TODO: Домашняя работа
    // 1. Перепишите все репозитории в приложении агента метрик,
    //    используя Dapper вместо сырого чтения данных.


    public class CpuMetricsRepository : ICpuMetricsRepository
    {
        //private const string ConnectionString = "Data Source=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";

        private readonly IOptions<DatabaseOptions> _databaseOptions;

        public CpuMetricsRepository(IOptions<DatabaseOptions> databaseOptions)
        {
            _databaseOptions = databaseOptions;
        }

        public void Create(CpuMetric item)
        {
            /*using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            // Создаём команду
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на вставку данных
            cmd.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(@value, @time)";
            // Добавляем параметры в запрос из нашего объекта
            cmd.Parameters.AddWithValue("@value", item.Value);
            // В таблице будем хранить время в секундах, поэтому преобразуем перед записью в секунды
            // через свойство
            cmd.Parameters.AddWithValue("@time", item.Time);
            // подготовка команды к выполнению
            cmd.Prepare();
            // Выполнение команды
            cmd.ExecuteNonQuery();*/
            DatabaseOptions databaseOptions = _databaseOptions.Value;
            using var connection = new SQLiteConnection(databaseOptions.ConnectionString);
            // Запрос на добавление данных с плейсхолдерами для параметров
            connection.Execute("INSERT INTO cpumetrics(value, time) VALUES(@value, @time)",
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
            /*using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            using var cmd = new SQLiteCommand(connection);
            // Прописываем в команду SQL-запрос на удаление данных
            cmd.CommandText = "DELETE FROM cpumetrics WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();*/
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            connection.Execute("DELETE FROM cpumetrics WHERE id=@id",new { id = id });
        }
        public void Update(CpuMetric item)
        {
            //using var connection = new SQLiteConnection(ConnectionString);
            //using var cmd = new SQLiteCommand(connection);
            //// Прописываем в команду SQL-запрос на обновление данных
            //cmd.CommandText = "UPDATE cpumetrics SET value = @value, time = @time WHERE id = @id; ";
            //cmd.Parameters.AddWithValue("@id", item.Id);
            //cmd.Parameters.AddWithValue("@value", item.Value);
            //cmd.Parameters.AddWithValue("@time", item.Time);
            //cmd.Prepare();
            //cmd.ExecuteNonQuery();
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            connection.Execute("UPDATE cpumetrics SET value = @value, time = @time WHERE id = @id",
            new
            {
                value = item.Value,
                time = item.Time,
                id = item.Id
            });
        }
        public IList<CpuMetric> GetAll()
        {
            //using var connection = new SQLiteConnection(ConnectionString);
            //connection.Open();
            //using var cmd = new SQLiteCommand(connection);
            //// Прописываем в команду SQL-запрос на получение всех данных из таблицы
            //cmd.CommandText = "SELECT * FROM cpumetrics";
            //var returnList = new List<CpuMetric>();
            //using (SQLiteDataReader reader = cmd.ExecuteReader())
            //{
            //    // Пока есть что читать — читаем
            //    while (reader.Read())
            //    {
            //        // Добавляем объект в список возврата
            //        returnList.Add(new CpuMetric
            //        {
            //            Id = reader.GetInt32(0),
            //            Value = reader.GetInt32(1),
            //            Time = reader.GetInt32(2)
            //        });
            //    }
            //}
            //return returnList;
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            //Id, Time, Value
            List<CpuMetric> metrics = connection.Query<CpuMetric>("SELECT * FROM cpumetrics").ToList();
            return metrics;
        }
        public CpuMetric GetById(int id)
        {
            //using var connection = new SQLiteConnection(ConnectionString);
            //connection.Open();
            //using var cmd = new SQLiteCommand(connection);
            //cmd.CommandText = "SELECT * FROM cpumetrics WHERE id=@id";
            //using (SQLiteDataReader reader = cmd.ExecuteReader())
            //{
            //    // Если удалось что-то прочитать
            //    if (reader.Read())
            //    {
            //        // возвращаем прочитанное
            //        return new CpuMetric
            //        {
            //            Id = reader.GetInt32(0),
            //            Value = reader.GetInt32(1),
            //            Time = reader.GetInt32(2)
            //        };
            //    }
            //    else
            //    {
            //        // Не нашлась запись по идентификатору, не делаем ничего
            //        return null;
            //    }
            //}
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            CpuMetric metric = connection.QuerySingle<CpuMetric>("SELECT Id, Time, Value FROM cpumetrics WHERE id = @id",
            new { id = id });
            return metric;
        }

        /// <summary>
        /// Получение данных по нагрузке на ЦП за период
        /// </summary>
        /// <param name="timeFrom">Время начала периода</param>
        /// <param name="timeTo">Время окончания периода</param>
        /// <returns></returns>
        public IList<CpuMetric> GetByTimePeriod(TimeSpan timeFrom, TimeSpan timeTo)
        {
            //using var connection = new SQLiteConnection(ConnectionString);
            //connection.Open();
            //using var cmd = new SQLiteCommand(connection);
            //// Прописываем в команду SQL-запрос на получение всех данных за период из таблицы
            //cmd.CommandText = "SELECT * FROM cpumetrics where time >= @timeFrom and time <= @timeTo";
            //cmd.Parameters.AddWithValue("@timeFrom", timeFrom.TotalSeconds);
            //cmd.Parameters.AddWithValue("@timeTo", timeTo.TotalSeconds);
            //var returnList = new List<CpuMetric>();
            //using (SQLiteDataReader reader = cmd.ExecuteReader())
            //{
            //    // Пока есть что читать — читаем
            //    while (reader.Read())
            //    {
            //        // Добавляем объект в список возврата
            //        returnList.Add(new CpuMetric
            //        {
            //            Id = reader.GetInt32(0),
            //            Value = reader.GetInt32(1),
            //            Time = reader.GetInt32(2)
            //        });
            //    }
            //}
            //return returnList;
            using var connection = new SQLiteConnection(_databaseOptions.Value.ConnectionString);
            string table = "cpumetrics";
            List<CpuMetric> metrics = connection.Query<CpuMetric>($"SELECT * FROM {table} where time >= @timeFrom and time <= @timeTo",
                new { timeFrom = timeFrom.TotalSeconds, timeTo = timeTo.TotalSeconds }).ToList();
            return metrics;
        }
    }
}
