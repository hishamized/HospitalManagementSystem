using HMS.Application.DTO.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IAnalyticsRepository
    {
        Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync();
    }
}
