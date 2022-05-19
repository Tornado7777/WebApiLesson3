using MetricsAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class RamMetricsAgentTests
    {

        private RamMetricsController _ramMetricsController;

        public RamMetricsAgentTests()
        {
            _ramMetricsController = new RamMetricsController();
        }

        [Fact]

        // TODO: Домашнее задание [Пункт 3]
        //  Добавьте проект с тестами для агента сбора метрик. Напишите простые Unit-тесты на каждый
        // метод отдельно взятого контроллера в обоих тестовых проектах.

        // На данный момент можно просто воспользоваться заглушками (как в проекте MetricsManagerTests)

        public void GetMetricsFromAgent_ReturnOk()
        {
            TimeSpan fromTime = TimeSpan.FromSeconds(0);
            TimeSpan toTime = TimeSpan.FromSeconds(100);

            IActionResult result = _ramMetricsController.GetMetrics(fromTime, toTime);

            Assert.IsAssignableFrom<IActionResult>(result);


        }
    }
}
