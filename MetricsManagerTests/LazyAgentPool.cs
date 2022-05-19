using MetricsManager.Models;
using System;

namespace MetricsManagerTests
{
    public class LazyAgentPool
    {

        // Lazy<T> - Обеспечивает поддержку отложенной инициализации.
        private static readonly Lazy<AgentPool> _instance =
            new Lazy<AgentPool>(() => new AgentPool());

        public LazyAgentPool() { }

        public static AgentPool Instance
        {
            get { return _instance.Value; }
            // Отложенная инициализация производится при первом обращении к свойству Lazy<T>.Value
        }
    }
}
