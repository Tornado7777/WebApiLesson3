using MetricsAgent.Controllers;
using MetricsAgent.Models;
using MetricsAgent.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class DotNetMetricsAgentTests
    {


        private DotNetMetricsController _dotNetMetricsController;
        private Mock<IDotNetMetricsRepository> mock;


        public DotNetMetricsAgentTests()
        {
            mock = new Mock<IDotNetMetricsRepository>();
            var mockLogger = new Mock<ILogger<DotNetMetricsController>>();
            _dotNetMetricsController = new DotNetMetricsController(mockLogger.Object, mock.Object);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnOk()
        {
            TimeSpan fromTime = TimeSpan.FromSeconds(0);
            TimeSpan toTime = TimeSpan.FromSeconds(100);
            mock.Setup(repository =>
                    repository.Create(It.IsAny<DotNetMetric>())).Verifiable();

            // ��������� �������� �� �����������
            var result = _dotNetMetricsController.Create(new
            MetricsAgent.Models.Requests.DotNetMetricCreateRequest
            {
                Time = TimeSpan.FromSeconds(1),
                Value = 50
            });
            // ��������� �������� �� ��, ��� ���� ������� ����������
            // �������� ����� Create ����������� � ������ ����� ������� � ���������
            mock.Verify(repository => repository.Create(It.IsAny<DotNetMetric>()),
            Times.AtMostOnce());

        }
    }
}