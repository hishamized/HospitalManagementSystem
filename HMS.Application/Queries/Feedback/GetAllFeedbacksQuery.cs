using HMS.Application.DTO.Feedback;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Feedback
{
    public class GetAllFeedbacksQuery : IRequest<IEnumerable<FeedbackListDto>>
    {
        // No parameters needed for now
        // If you want to add filters later (e.g. by DoctorId or DateRange), you can add them here.
    }
}
