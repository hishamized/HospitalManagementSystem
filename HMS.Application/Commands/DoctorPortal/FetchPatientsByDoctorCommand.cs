using HMS.Application.DTO.DoctorPortal;
using HMS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.DoctorPortal
{
    public class FetchPatientsByDoctorCommand : IRequest<IEnumerable<FetchPatientsByDoctorDto>>
    {
        public int DoctorId;
        public FetchPatientsByDoctorCommand(int doctorId) {
            DoctorId = doctorId;
        }
    }
}
