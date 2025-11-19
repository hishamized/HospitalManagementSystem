using FluentValidation;
using HMS.Application.Commands.LabTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.LabTest
{
    public class AddLabTestCommandValidator : AbstractValidator<AddLabTestCommand>
    {
        public AddLabTestCommandValidator()
        {
            // Test Name Validation
            RuleFor(x => x.dto.TestName)
                .NotEmpty()
                .WithMessage("Test Name is required.")
                .MinimumLength(3)
                .WithMessage("Test Name should be at least 3 characters.")
                .MaximumLength(150)
                .WithMessage("Test Name cannot exceed 150 characters.");

            // Description Validation (Optional)
            RuleFor(x => x.dto.Description)
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.dto.Description));

            // Sample Type Validation (Optional)
            RuleFor(x => x.dto.SampleType)
                .MaximumLength(50)
                .WithMessage("Sample Type cannot exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.dto.SampleType));

            // Normal Range Validation
            RuleFor(x => x.dto.NormalRange)
                .NotEmpty()
                .WithMessage("Normal Range is required.")
                .MaximumLength(200)
                .WithMessage("Normal Range cannot exceed 200 characters.");

            // Price Validation (Optional)
            RuleFor(x => x.dto.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.")
                .LessThanOrEqualTo(999999.99m)
                .WithMessage("Price cannot exceed 999,999.99.")
                .When(x => x.dto.Price.HasValue);

            // CreatedAt Validation
            RuleFor(x => x.dto.CreatedAt)
                .NotEmpty()
                .WithMessage("Created date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                .WithMessage("Created date cannot be in the future.");

            // UpdatedAt Validation (Optional)
            RuleFor(x => x.dto.UpdatedAt)
                .GreaterThan(x => x.dto.CreatedAt)
                .WithMessage("Updated date must be after Created date.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                .WithMessage("Updated date cannot be in the future.")
                .When(x => x.dto.UpdatedAt.HasValue);

            // IsActive Validation (always valid as it's a boolean, but can add custom business rules)
            RuleFor(x => x.dto.IsActive)
                .NotNull()
                .WithMessage("Active status must be specified.");
        }
    }
}