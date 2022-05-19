using MetricsAgent.Models;
using MetricsAgent.Services;

namespace MetricsAgent.Controllers
{
    public interface ICpuMetricsRepository : IRepository<CpuMetric>
    {
    }
}
