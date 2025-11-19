using FluentValidation;
using HMS.Application.Commands.LabTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.LabTest
{
    public class EditLabTestCommandValidator : AbstractValidator<EditLabTestCommand>
    {
        public EditLabTestCommandValidator() {
            // Test Name Validation
            RuleFor(x => x.Dto.TestName)
                    .NotEmpty()
                    .WithMessage("Test Name is required.")
                    .MinimumLength(3)
                    .WithMessage("Test Name should be at least 3 characters.")
                    .MaximumLength(150)
                    .WithMessage("Test Name cannot exceed 150 characters.");

            // Description Validation (Optional)
            RuleFor(x => x.Dto.Description)
                    .MaximumLength(1000)
                    .WithMessage("Description cannot exceed 1000 characters.")
                    .When(x => !string.IsNullOrEmpty(x.Dto.Description));

            // Sample Type Validation (Optional)
            RuleFor(x => x.Dto.SampleType)
                    .MaximumLength(50)
                    .WithMessage("Sample Type cannot exceed 50 characters.")
                    .When(x => !string.IsNullOrEmpty(x.Dto.SampleType));

            // Normal Range Validation
            RuleFor(x => x.Dto.NormalRange)
                    .NotEmpty()
                    .WithMessage("Normal Range is required.")
                    .MaximumLength(200)
                    .WithMessage("Normal Range cannot exceed 200 characters.");

            // Price Validation (Optional)
            RuleFor(x => x.Dto.Price)
                    .GreaterThan(0)
                    .WithMessage("Price must be greater than 0.")
                    .LessThanOrEqualTo(999999.99m)
                    .WithMessage("Price cannot exceed 999,999.99.")
                    .When(x => x.Dto.Price.HasValue);

            // UpdatedAt Validation (Optional)
            RuleFor(x => x.Dto.UpdatedAt)
                    .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                    .WithMessage("Updated date cannot be in the future.")
                    .When(x => x.Dto.UpdatedAt.HasValue);

            // IsActive Validation (always valid as it's a boolean, but can add custom business rules)
            RuleFor(x => x.Dto.IsActive)
                    .NotNull()
                    .WithMessage("Active status must be specified.");
        }
    }
}
