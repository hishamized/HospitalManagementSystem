using HMS.Application.DTO.DoctorDocument;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.DoctorDocument
{
    public class GetDoctorDocumentsByDoctorIdCommand : IRequest<IEnumerable<GetDoctorDocumentsDto>>
    {
        public int DoctorId { get; set; }

        public GetDoctorDocumentsByDoctorIdCommand(int doctorId)
        {
            DoctorId = doctorId;
        }
    }
}

