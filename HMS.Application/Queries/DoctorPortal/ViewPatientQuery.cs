using HMS.Application.DTO.DoctorPortal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.DoctorPortal
{
    public class ViewPatientQuery : IRequest<ViewPatientDto>
    {
        public int DoctorId;
        public int PatientId;
        public ViewPatientQuery(int doctorId, int patientId) {
            DoctorId = doctorId;
            PatientId = patientId;
        }
    }
}
