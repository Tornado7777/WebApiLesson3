using System.Collections.Generic;

namespace MetricsAgent.Models.Requests
{
    public class AllCpuMetricsResponse
    {
        public List<CpuMetricDto> Metrics { get; set; }
    }
}
