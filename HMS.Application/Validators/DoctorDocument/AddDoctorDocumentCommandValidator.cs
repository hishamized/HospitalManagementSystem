using FluentValidation;
using HMS.Application.Commands.DoctorDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.DoctorDocument
{
    public class AddDoctorDocumentCommandValidator : AbstractValidator<AddDoctorDocumentCommand>
    {
        public AddDoctorDocumentCommandValidator()
        {
            RuleFor(x => x.DoctorDocument.DoctorId)
                .GreaterThan(0).WithMessage("DoctorId is required.");

            RuleFor(x => x.DoctorDocument.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .MaximumLength(255);

            RuleFor(x => x.DoctorDocument.FileType)
                .NotEmpty().WithMessage("File type is required.")
                .MaximumLength(50)
                .Must(type => new[] { "pdf", "jpg", "jpeg", "png", "docx" }
                    .Contains(type.ToLower()))
                .WithMessage("Only PDF, JPG, JPEG, PNG, or DOCX files are allowed.");

            RuleFor(x => x.DoctorDocument.FileSize)
                .LessThanOrEqualTo(500 * 1024)
                .WithMessage("File size must be less than or equal to 500 KB.");

            RuleFor(x => x.DoctorDocument.UploadedBy)
                .NotEmpty()
                .WithMessage("UploadedBy is required.")
                .MaximumLength(100);
        }
    }
}
