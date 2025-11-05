using HMS.Application.DTO.Doctor;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Doctor
{
    public class GetDoctorRoundsByPatientQuery : IRequest<IEnumerable<DoctorRoundHistoryDto>>
    {
        public int PatientId { get; set; }

        public GetDoctorRoundsByPatientQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
