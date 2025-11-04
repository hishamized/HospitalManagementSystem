using FluentValidation;
using HMS.Application.Commands.Bed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Bed
{
    public class AddBedCommandValidator : AbstractValidator<AddBedCommand>
    {
        public AddBedCommandValidator()
        {
            RuleFor(x => x.Bed.BedCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Bed.BedNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Bed.WardId).GreaterThan(0);
            RuleFor(x => x.Bed.BedType).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Bed.Status).NotEmpty().MaximumLength(50);
        }
    }
}
