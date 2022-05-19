using System.Collections.Generic;

namespace MetricsAgent.Models.Requests
{
    public class AllDotNetMetricsResponse
    {
        public List<DotNetMetricDto> Metrics { get; set; }
    }
}
