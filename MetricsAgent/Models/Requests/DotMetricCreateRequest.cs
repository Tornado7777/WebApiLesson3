using System;

namespace MetricsAgent.Models.Requests
{
    public class DotMetricCreateRequest
    {
        
            public TimeSpan Time { get; set; }
            public int Value { get; set; }

    }
}
