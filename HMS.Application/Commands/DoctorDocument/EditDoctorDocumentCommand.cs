using HMS.Application.DTO.DoctorDocument;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.DoctorDocument
{
    public class EditDoctorDocumentCommand : IRequest<bool>
    {
        public string? UploadedBy;

        public EditDoctorDocumentDto Dto { get; set; } = null!;
        public IFormFile File { get; set; } = null!;
        public string OldFilePath { get; set; } = string.Empty; // new


    }
}
