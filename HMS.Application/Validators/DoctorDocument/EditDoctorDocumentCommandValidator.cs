using FluentValidation;
using HMS.Application.Commands.DoctorDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.DoctorDocument
{
    public class EditDoctorDocumentCommandValidator : AbstractValidator<EditDoctorDocumentCommand>
    {
        public EditDoctorDocumentCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .GreaterThan(0)
                .WithMessage("Document ID is required.");

            RuleFor(x => x.Dto.DoctorId)
                .GreaterThan(0)
                .WithMessage("Doctor ID is required.");

            RuleFor(x => x.File)
                .NotNull().WithMessage("Please upload a file.")
                .Must(f => f.Length <= 500 * 1024)
                    .WithMessage("File size must be less than 500 KB.")
                .Must(f =>
                {
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
                    var ext = Path.GetExtension(f.FileName)?.ToLower();
                    return allowedExtensions.Contains(ext);
                })
                    .WithMessage("Only PDF, JPG, PNG, DOC, and DOCX files are allowed.");
        }
    }
}
