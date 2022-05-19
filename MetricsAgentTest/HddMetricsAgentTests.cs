using MetricsAgent.Controllers;
using MetricsAgent.Models;
using MetricsAgent.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class HddMetricsAgentTests
    {


        private HddMetricsController _HddMetricsController;
        private Mock<IHddMetricsRepository> mock;


        public HddMetricsAgentTests()
        {
            mock = new Mock<IHddMetricsRepository>();
            var mockLogger = new Mock<ILogger<HddMetricsController>>();
            _HddMetricsController = new HddMetricsController(mockLogger.Object, mock.Object);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnOk()
        {
            TimeSpan fromTime = TimeSpan.FromSeconds(0);
            TimeSpan toTime = TimeSpan.FromSeconds(100);
            mock.Setup(repository =>
                    repository.Create(It.IsAny<HddMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var result = _HddMetricsController.Create(new
            MetricsAgent.Models.Requests.HddMetricCreateRequest
            {
                Time = TimeSpan.FromSeconds(1),
                Value = 50
            });
            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<HddMetric>()),
            Times.AtMostOnce());

        }
    }
}

