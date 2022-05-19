using System.Collections.Generic;

namespace MetricsAgent.Models.Requests
{
    public class AllNetworkMetricsResponse
    {
        public List<NetworkMetricDto> Metrics { get; set; }
    }
}

