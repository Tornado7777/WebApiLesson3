using MetricsManager.Controllers;
using MetricsManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;
using Xunit.Priority;

namespace MetricsManagerTests
{
    public class AgentsControllerTests
    {

        private AgentsController _agentsController;
        private AgentPool _agentPool;

        public AgentsControllerTests()
        {
            _agentPool = LazyAgentPool.Instance;
            _agentsController = new AgentsController(_agentPool);
        }


        [Theory, Priority(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public void RegisterAgentTest(int agentId)
        {
            AgentInfo agentInfo = new AgentInfo() { AgentId = agentId, Enable = true };
            IActionResult actionResult = _agentsController.RegisterAgent(agentInfo);
            Assert.IsAssignableFrom<IActionResult>(actionResult);
        }

        [Fact, Priority(2)]
        public void GetAgentsTest()
        {
            IActionResult actionResult = _agentsController.GetAllAgents();
            OkObjectResult result = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Assert.NotNull(result.Value as IEnumerable<AgentInfo>);
            Assert.NotEmpty((IEnumerable<AgentInfo>)result.Value);
        }

    }
}
