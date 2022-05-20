using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent
{
    public class Program
    {

        public static void Main(string[] args)
        {
            NLog.Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception) // Обработка всех исключений, в ходе работы приложения
            {
                // Фиксирование исключений в лог
                logger.Error(exception, "Stopped program because of exception");
                // Возбуждение исключения, завершение работы сервиса
                throw;
            }
            finally
            {
                // Завершение работы логера
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                }).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });
    }
}
