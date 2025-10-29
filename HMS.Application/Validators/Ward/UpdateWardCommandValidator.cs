using FluentValidation;
using HMS.Application.Commands.Ward;

namespace HMS.Application.Validators.Ward
{
    public class UpdateWardCommandValidator : AbstractValidator<UpdateWardCommand>
    {
        public UpdateWardCommandValidator()
        {
            // Ensure the nested Ward object exists before validating its fields
            When(x => x.Ward != null, () =>
            {
                RuleFor(x => x.Ward.Id)
                    .GreaterThan(0)
                    .WithMessage("Ward Id must be greater than 0.");

                RuleFor(x => x.Ward.WardCode)
                    .NotEmpty().WithMessage("Ward Code is required.")
                    .MaximumLength(20).WithMessage("Ward Code cannot exceed 20 characters.");

                RuleFor(x => x.Ward.WardName)
                    .NotEmpty().WithMessage("Ward Name is required.")
                    .MaximumLength(100).WithMessage("Ward Name cannot exceed 100 characters.");

                RuleFor(x => x.Ward.WardType)
                    .NotEmpty().WithMessage("Ward Type is required.");

                RuleFor(x => x.Ward.Capacity)
                    .GreaterThan(0).WithMessage("Capacity must be greater than 0.");

                RuleFor(x => x.Ward.OccupiedBeds)
                    .GreaterThanOrEqualTo(0).WithMessage("Occupied beds cannot be negative.")
                    .LessThanOrEqualTo(x => x.Ward.Capacity)
                    .WithMessage("Occupied beds cannot exceed capacity.");

                RuleFor(x => x.Ward.Location)
                    .NotEmpty().WithMessage("Location is required.");

                RuleFor(x => x.Ward.Description)
                    .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
            });

            // Validate that Ward object itself is not null
            RuleFor(x => x.Ward)
                .NotNull()
                .WithMessage("Ward object cannot be null.");
        }
    }
}
