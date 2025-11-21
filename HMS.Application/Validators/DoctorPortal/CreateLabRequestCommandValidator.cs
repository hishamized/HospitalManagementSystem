using FluentValidation;
using HMS.Application.Commands.DoctorPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.DoctorPortal
{
    public class CreateLabRequestCommandValidator : AbstractValidator<CreateLabRequestCommand>
    {
        public CreateLabRequestCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request payload cannot be null.");

            RuleFor(x => x.Request.LabRequest)
                .NotNull().WithMessage("LabRequest data is required.");

            RuleFor(x => x.Request.LabRequest.PatientId)
                .GreaterThan(0).WithMessage("PatientId is required.");

            RuleFor(x => x.Request.LabRequest.RequestDate)
                .NotEmpty().WithMessage("RequestDate is required.");

            RuleFor(x => x.Request.LabRequest.Status)
                .NotEmpty().WithMessage("Status is required.");

            RuleFor(x => x.Request.LabRequest.Notes)
                .NotEmpty().WithMessage("Notes are required.");

            RuleFor(x => x.Request.LabRequest.CreatedAt)
                .NotEmpty().WithMessage("CreatedAt is required.");

            RuleFor(x => x.Request.LabRequest.IsActive)
                .NotNull().WithMessage("IsActive must be specified.");


            // ---------- VALIDATE THE ITEMS LIST ----------
            RuleFor(x => x.Request.LabRequestItems)
                .NotNull().WithMessage("LabRequestItems cannot be null.")
                .Must(list => list.Any()).WithMessage("At least one LabRequestItem is required.");

            RuleForEach(x => x.Request.LabRequestItems)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.LabTestId)
                        .GreaterThan(0).WithMessage("LabTestId is required.");

                    item.RuleFor(i => i.Status)
                        .NotEmpty().WithMessage("Item Status is required.");

                    item.RuleFor(i => i.CreatedAt)
                        .NotEmpty().WithMessage("CreatedAt is required for each item.");

                    item.RuleFor(i => i.IsActive)
                        .NotNull().WithMessage("IsActive must be specified for each item.");
                });
        }
    }
}
