using System;

namespace MetricsAgent.Models.Requests
{
    public class DotNetMetricCreateRequest
    {
        
            public TimeSpan Time { get; set; }
            public int Value { get; set; }

    }
}
