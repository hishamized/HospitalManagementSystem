using FluentValidation;
using HMS.Application.Commands.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.User
{
    public class VerifyEmailPhoneCommandValidator : AbstractValidator<VerifyEmailPhoneCommand>
    {
        public VerifyEmailPhoneCommandValidator()
        {
            RuleFor(x => x.VerifyEmailPhoneDto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.VerifyEmailPhoneDto.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{1,3}\s?\d{8,12}$")
            .WithMessage("Invalid phone number format. Use format like +91 7889466366.");
        }
    }
}
