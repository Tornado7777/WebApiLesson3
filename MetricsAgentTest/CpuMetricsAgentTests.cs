using MetricsAgent.Controllers;
using MetricsAgent.Models;
using Microsoft.Extensions.Logging;
using MetricsAgent.Services;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class CpuMetricsAgentTests
    {


        private CpuMetricsController _cpuMetricsController;
        private Mock<ICpuMetricsRepository> mock;


        public CpuMetricsAgentTests()
        {
            mock = new Mock<ICpuMetricsRepository>();
            var mockLogger = new Mock<ILogger<CpuMetricsController>>();
            _cpuMetricsController = new CpuMetricsController(mockLogger.Object, mock.Object);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnOk()
        {
            TimeSpan fromTime = TimeSpan.FromSeconds(0);
            TimeSpan toTime = TimeSpan.FromSeconds(100);
            mock.Setup(repository =>
                    repository.Create(It.IsAny<CpuMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var result = _cpuMetricsController.Create(new
            MetricsAgent.Models.Requests.CpuMetricCreateRequest
            {
                Time = TimeSpan.FromSeconds(1),
                Value = 50
            });
            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<CpuMetric>()),
            Times.AtMostOnce());

        }
    }
}
