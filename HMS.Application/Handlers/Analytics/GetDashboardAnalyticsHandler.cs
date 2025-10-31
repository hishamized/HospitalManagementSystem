using HMS.Application.DTO.Analytics;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Analytics;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Analytics
{
    public class GetDashboardAnalyticsHandler : IRequestHandler<GetDashboardAnalyticsQuery, DashboardAnalyticsDto>
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public GetDashboardAnalyticsHandler(IAnalyticsRepository AnalyticsRepository)
        {
            _analyticsRepository = AnalyticsRepository;
        }

        public async Task<DashboardAnalyticsDto> Handle(GetDashboardAnalyticsQuery request, CancellationToken cancellationToken)
        {
            // Fetch analytics data from repository
            var analyticsData = await _analyticsRepository.GetDashboardAnalyticsAsync();

            // Return the aggregated result to the controller
            return analyticsData;
        }
    }
}
