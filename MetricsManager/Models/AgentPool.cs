using MetricsManager.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace MetricsManager.Models
{
    public class AgentPool : IAgentPool<AgentInfo>
    {
        private Dictionary<int, AgentInfo> _values;

        public AgentPool()
        {
            _values = new Dictionary<int, AgentInfo>();
        }

        public void Add(AgentInfo value)
        {
            if (!_values.ContainsKey(value.AgentId))
                _values.Add(value.AgentId, value);
        }

        public AgentInfo[] Get()
        {

            return _values.Values.ToArray();
        }

        public Dictionary<int, AgentInfo> Values
        {
            get { return _values; }
            set { _values = value; }
        }
    }
}
