using FluentValidation;
using HMS.Application.Commands.Slot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Slot
{
    public class AddSlotCommandValidator : AbstractValidator<AddSlotCommand>
    {
        public AddSlotCommandValidator()
        {
            RuleFor(x => x.Slot.ReportingTime)
                .NotEmpty().WithMessage("Reporting time is required.");

            RuleFor(x => x.Slot.LeavingTime)
                .NotEmpty().WithMessage("Leaving time is required.")
                .GreaterThan(x => x.Slot.ReportingTime)
                .WithMessage("Leaving time must be after reporting time.");

            RuleFor(x => x.Slot.DaysOfWeek)
                .GreaterThan(0).WithMessage("At least one day must be selected.");
        }
    }
}
