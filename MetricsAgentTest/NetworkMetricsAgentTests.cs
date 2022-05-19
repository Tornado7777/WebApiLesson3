using MetricsAgent.Controllers;
using MetricsAgent.Models;
using Microsoft.Extensions.Logging;
using MetricsAgent.Services;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class NetworkMetricsAgentTests
    {


        private NetworkMetricsController _networkMetricsController;
        private Mock<INetworkMetricsRepository> mock;


        public NetworkMetricsAgentTests()
        {
            mock = new Mock<INetworkMetricsRepository>();
            var mockLogger = new Mock<ILogger<NetworkMetricsController>>();
            _networkMetricsController = new NetworkMetricsController(mockLogger.Object, mock.Object);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnOk()
        {
            TimeSpan fromTime = TimeSpan.FromSeconds(0);
            TimeSpan toTime = TimeSpan.FromSeconds(100);
            mock.Setup(repository =>
                    repository.Create(It.IsAny<NetworkMetric>())).Verifiable();

            // Выполняем действие на контроллере
            var result = _networkMetricsController.Create(new
            MetricsAgent.Models.Requests.NetworkMetricCreateRequest
            {
                Time = TimeSpan.FromSeconds(1),
                Value = 50
            });
            // Проверяем заглушку на то, что пока работал контроллер
            // Вызвался метод Create репозитория с нужным типом объекта в параметре
            mock.Verify(repository => repository.Create(It.IsAny<NetworkMetric>()),
            Times.AtMostOnce());

        }
    }
}
