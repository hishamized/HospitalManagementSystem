using FluentValidation;
using HMS.Application.Commands.Slot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Slot
{
    public class EditSlotCommandValidator : AbstractValidator<EditSlotCommand>
    {
        public EditSlotCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .GreaterThan(0)
                .WithMessage("Invalid Slot ID.");

            RuleFor(x => x.Dto.ReportingTime)
                .NotEmpty()
                .WithMessage("Reporting time is required.");

            RuleFor(x => x.Dto.LeavingTime)
                .NotEmpty()
                .WithMessage("Leaving time is required.");

            RuleFor(x => x.Dto.DaysOfWeek)
                .InclusiveBetween(1, 127)
                .WithMessage("At least one valid day must be selected.");
        }
    }
}
