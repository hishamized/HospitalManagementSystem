using FluentValidation;
using HMS.Application.Commands.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Department
{
    public class AddDepartmentCommandValidator : AbstractValidator<AddDepartmentCommand>
    {
        public AddDepartmentCommandValidator()
        {
            // Validate the nested DTO
            RuleFor(x => x.DepartmentDto).NotNull().WithMessage("Department data is required.");

            // Department Name is required and max 100 characters
            RuleFor(x => x.DepartmentDto.Name)
                .NotEmpty().WithMessage("Department Name is required.")
                .MaximumLength(100).WithMessage("Department Name cannot exceed 100 characters.");

            // Description is optional, but max 255 characters
            RuleFor(x => x.DepartmentDto.Description)
                .MaximumLength(255).WithMessage("Description cannot exceed 255 characters.");
        }
    }
}
