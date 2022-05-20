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

            CreateMap<DotNetMetric, DotNetMetricDto>().
                ForMember(x => x.Time, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.Time)));

            CreateMap<HddMetric, HddMetricDto>().
                ForMember(x => x.Time, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.Time)));

            CreateMap<NetworkMetric, NetworkMetricDto>().
                ForMember(x => x.Time, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.Time)));

            CreateMap<RamMetric, RamMetricDto>().
                ForMember(x => x.Time, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.Time)));
        }
    }
}
