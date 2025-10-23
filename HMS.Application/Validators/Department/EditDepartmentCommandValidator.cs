using FluentValidation;
using HMS.Application.Commands.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Department
{
    public class EditDepartmentCommandValidator : AbstractValidator<EditDepartmentCommand>
    {
        public EditDepartmentCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .GreaterThan(0)
                .WithMessage("Department ID is required for editing.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");

            RuleFor(x => x.Dto.Description)
                .MaximumLength(255)
                .WithMessage("Description cannot exceed 255 characters.");
        }
    }
}
