using HMS.Application.DTO.PatientPortal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.PatientPortal
{
    public class SearchPatientByIdentifierQuery : IRequest<GetPatientByIdentifierDto>
    {
        public string Identifier { get; set; }
        public SearchPatientByIdentifierQuery(string identifier) { 
               Identifier = identifier;
        }
    }
}
