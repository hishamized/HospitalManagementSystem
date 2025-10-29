using FluentValidation;
using HMS.Application.DTO.Ward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Ward
{
    public class CreateWardValidator : AbstractValidator<CreateWardDto>
    {
        public CreateWardValidator()
        {
            RuleFor(x => x.WardCode)
                .NotEmpty().WithMessage("Ward code is required.")
                .MaximumLength(50).WithMessage("Ward code cannot exceed 50 characters.");

            RuleFor(x => x.WardName)
                .NotEmpty().WithMessage("Ward name is required.")
                .MaximumLength(100).WithMessage("Ward name cannot exceed 100 characters.");

            RuleFor(x => x.WardType)
                .NotEmpty().WithMessage("Ward type is required.")
                .MaximumLength(50).WithMessage("Ward type cannot exceed 50 characters.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

            RuleFor(x => x.OccupiedBeds)
                .GreaterThanOrEqualTo(0).WithMessage("Occupied beds cannot be negative.")
                .LessThanOrEqualTo(x => x.Capacity)
                .WithMessage("Occupied beds cannot exceed capacity.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.");
        }
    }
}
