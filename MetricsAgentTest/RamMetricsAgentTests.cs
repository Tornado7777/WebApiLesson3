using MetricsAgent.Controllers;
using MetricsAgent.Models;
using Microsoft.Extensions.Logging;
using MetricsAgent.Services;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class RamMetricsAgentTests
    {


        private RamMetricsController _ramMetricsController;
        private Mock<IRamMetricsRepository> mock;


        public RamMetricsAgentTests()
        {
            mock = new Mock<IRamMetricsRepository>();
            var mockLogger = new Mock<ILogger<RamMetricsController>>();
            _ramMetricsController = new RamMetricsController(mockLogger.Object, mock.Object);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnOk()
        {
            TimeSpan fromTime = TimeSpan.FromSeconds(0);
            TimeSpan toTime = TimeSpan.FromSeconds(100);
            mock.Setup(repository =>
                    repository.Create(It.IsAny<RamMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var result = _ramMetricsController.Create(new
            MetricsAgent.Models.Requests.RamMetricCreateRequest
            {
                Time = TimeSpan.FromSeconds(1),
                Value = 50
            });
            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<RamMetric>()),
            Times.AtMostOnce());

        }
    }
}
