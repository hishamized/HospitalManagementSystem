using FluentValidation;
using HMS.Application.Commands.Bed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Bed
{
    public class EditBedValidator : AbstractValidator<EditBedCommand>
    {
        public EditBedValidator()
        {
            RuleFor(x => x.Bed.Id)
                .GreaterThan(0).WithMessage("Invalid Bed ID.");

            RuleFor(x => x.Bed.BedCode)
                .NotEmpty().WithMessage("Bed code is required.")
                .MaximumLength(50).WithMessage("Bed code cannot exceed 50 characters.");

            RuleFor(x => x.Bed.BedNumber)
                .NotEmpty().WithMessage("Bed number is required.")
                .MaximumLength(50).WithMessage("Bed number cannot exceed 50 characters.");

            RuleFor(x => x.Bed.BedType)
                .NotEmpty().WithMessage("Bed type is required.");

            RuleFor(x => x.Bed.Status)
                .NotEmpty().WithMessage("Status is required.");
        }
    }
}
