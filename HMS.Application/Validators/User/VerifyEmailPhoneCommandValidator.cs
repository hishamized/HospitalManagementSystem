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
             .Matches(@"^\+?[1-9]\d{1,14}$")
             .WithMessage("Invalid phone number format. It should include an optional '+' and up to 15 digits.");

        }
    }
}
