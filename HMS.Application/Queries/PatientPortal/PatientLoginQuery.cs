using HMS.Application.DTO.PatientPortal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.PatientPortal
{
    public class PatientLoginQuery : IRequest<PatientLoginResultDto>
    {
        public PatientLoginDto LoginDto { get; }

        public PatientLoginQuery(PatientLoginDto loginDto)
        {
            LoginDto = loginDto;
        }
    }
}
