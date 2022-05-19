using System.Collections.Generic;

namespace MetricsAgent.Models.Requests
{
    public class AllRamMetricsResponse
    {
        public List<RamMetricDto> Metrics { get; set; }
    }
}
