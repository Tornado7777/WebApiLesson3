using AutoMapper;
using MetricsAgent.Models;
using System;

namespace MetricsAgent
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CpuMetric, CpuMetricDto>().
                ForMember(x => x.Time, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.Time)));
        }
    }
}
