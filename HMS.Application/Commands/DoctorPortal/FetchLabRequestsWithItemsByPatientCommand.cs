using HMS.Application.DTO.DoctorPortal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.DoctorPortal
{
    public class FetchLabRequestsWithItemsByPatientCommand : IRequest<IEnumerable<FetchLabRequestsWithItemsByPatientDto>>
    {
        public int DoctorId;
        public int PatientId;
        public FetchLabRequestsWithItemsByPatientCommand(int doctorId, int patientId) {
            DoctorId = doctorId;
            PatientId = patientId;
        }
    }
}
