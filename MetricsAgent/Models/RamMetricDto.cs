﻿using System;

namespace MetricsAgent.Models
{
    public class RamMetricDto
    {
        public TimeSpan Time { get; set; }
        public int Value { get; set; }
        public int Id { get; set; }
    }
}
