using HMS.Application.DTO.DoctorPortal;
using HMS.Application.DTO.PatientPortal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.DoctorPortal
{
    public class DoctorLoginQuery : IRequest<DoctorLoginResultDto>
    {
        public DoctorLoginDto LoginDto { get; }

        public DoctorLoginQuery(DoctorLoginDto loginDto)
        {
            LoginDto = loginDto;
        }
    }
}
