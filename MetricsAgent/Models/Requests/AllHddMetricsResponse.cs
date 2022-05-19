using System.Collections.Generic;

namespace MetricsAgent.Models.Requests
{
    public class AllHddMetricsResponse
    {
        public List<HddMetricDto> Metrics { get; set; }
    }
}

