using FluentValidation;
using HMS.Application.Commands.Role;

namespace HMS.Application.Validators.Role
{
    public class EditRoleCommandValidator : AbstractValidator<EditRoleCommand>
    {
        public EditRoleCommandValidator()
        {
            RuleFor(x => x.Role.Id)
                .GreaterThan(0)
                .WithMessage("Invalid role ID.");

            RuleFor(x => x.Role.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters.");

            RuleFor(x => x.Role.Description)
                .NotEmpty().WithMessage("Role description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
