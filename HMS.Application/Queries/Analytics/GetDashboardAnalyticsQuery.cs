using HMS.Application.DTO.Analytics;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Analytics
{
    public class GetDashboardAnalyticsQuery : IRequest<DashboardAnalyticsDto>
    {
        // No input parameters required since the SP doesn’t take any arguments.
        // If in the future you want date ranges or filters, you can add them here.
    }
}
