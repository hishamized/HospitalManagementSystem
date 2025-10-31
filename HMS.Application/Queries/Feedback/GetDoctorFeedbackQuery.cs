using HMS.Application.DTO.Feedback;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Feedback
{

    public class GetDoctorFeedbackQuery : IRequest<IEnumerable<DoctorFeedbackDto>>
    {
        public int DoctorId { get; set; }

        public GetDoctorFeedbackQuery(int doctorId)
        {
            DoctorId = doctorId;
        }
    }
}
