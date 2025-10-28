using FluentValidation;

namespace HMS.Application.Features.Users.Commands.CreateAdmin
{
    public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
    {
        public CreateAdminCommandValidator()
        {
            RuleFor(x => x.Admin.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Admin.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Admin.ContactNumber)
                .NotEmpty().WithMessage("Contact Number is required.")
                .MaximumLength(20);

            RuleFor(x => x.Admin.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                .LessThan(DateTime.Now).WithMessage("Date of Birth cannot be in the future.");

            RuleFor(x => x.Admin.Gender)
                .NotEmpty().WithMessage("Gender is required.");

            RuleFor(x => x.Admin.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50);

            RuleFor(x => x.Admin.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.Admin.RoleId)
                .GreaterThan(0).WithMessage("Role selection is required.");
        }
    }
}
