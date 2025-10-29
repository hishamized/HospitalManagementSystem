using HMS.Application.DTO.DoctorDocument;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.DoctorDocument
{
    public class AddDoctorDocumentCommand : IRequest<int> // Returns new document Id
    {
        public DoctorDocumentDto DoctorDocument { get; set; }

        public AddDoctorDocumentCommand(DoctorDocumentDto doctorDocument)
        {
            DoctorDocument = doctorDocument;
        }
    }
}
