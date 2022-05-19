using MetricsAgent.Controllers;
using MetricsAgent.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MetricsAgent.Services
{
    public class CpuMetricsRepository : ICpuMetricsRepository
    {

            private const string ConnectionString = "Data Source=metrics.db;Version=3;Pooling=true;Max Pool Size=100;";


            public void Create(CpuMetric item)
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                // Создаём команду
                using var cmd = new SQLiteCommand(connection);
                // Прописываем в команду SQL-запрос на вставку данных
                cmd.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(@value, @time)";
                // Добавляем параметры в запрос из нашего объекта
                cmd.Parameters.AddWithValue("@value", item.Value);
                // В таблице будем хранить время в секундах, поэтому преобразуем перед записью в секунды
                // через свойство
                cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
                // подготовка команды к выполнению
                cmd.Prepare();
                // Выполнение команды
                cmd.ExecuteNonQuery();
            }
            public void Delete(int id)
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var cmd = new SQLiteCommand(connection);
                // Прописываем в команду SQL-запрос на удаление данных
                cmd.CommandText = "DELETE FROM cpumetrics WHERE id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            public void Update(CpuMetric item)
            {
                using var connection = new SQLiteConnection(ConnectionString);
                using var cmd = new SQLiteCommand(connection);
                // Прописываем в команду SQL-запрос на обновление данных
                cmd.CommandText = "UPDATE cpumetrics SET value = @value, time = @time WHERE id = @id; ";
                cmd.Parameters.AddWithValue("@id", item.Id);
                cmd.Parameters.AddWithValue("@value", item.Value);
                cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            public IList<CpuMetric> GetAll()
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var cmd = new SQLiteCommand(connection);
                // Прописываем в команду SQL-запрос на получение всех данных из таблицы
                cmd.CommandText = "SELECT * FROM cpumetrics";
                var returnList = new List<CpuMetric>();
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    // Пока есть что читать — читаем
                    while (reader.Read())
                    {
                        // Добавляем объект в список возврата
                        returnList.Add(new CpuMetric
                        {
                            Id = reader.GetInt32(0),
                            Value = reader.GetInt32(1),
                            // Налету преобразуем прочитанные секунды в метку времени
                            Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                        });
                    }
                }
                return returnList;
            }
            public CpuMetric GetById(int id)
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var cmd = new SQLiteCommand(connection);
                cmd.CommandText = "SELECT * FROM cpumetrics WHERE id=@id";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    // Если удалось что-то прочитать
                    if (reader.Read())
                    {
                        // возвращаем прочитанное
                        return new CpuMetric
                        {
                            Id = reader.GetInt32(0),
                            Value = reader.GetInt32(1),
                            Time = TimeSpan.FromSeconds(reader.GetInt32(1))
                        };
                    }
                    else
                    {
                        // Не нашлась запись по идентификатору, не делаем ничего
                        return null;
                    }
                }
            }
        }
}
