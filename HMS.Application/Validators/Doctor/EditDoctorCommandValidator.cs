using FluentValidation;
using HMS.Application.Commands.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Doctor
{
    public class EditDoctorCommandValidator : AbstractValidator<EditDoctorCommand>
    {
        public EditDoctorCommandValidator()
        {
            RuleFor(x => x.Doctor).NotNull().WithMessage("Doctor data must be provided.");

            RuleFor(x => x.Doctor.Id).GreaterThan(0).WithMessage("Invalid doctor ID.");

            RuleFor(x => x.Doctor.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

            RuleFor(x => x.Doctor.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

            RuleFor(x => x.Doctor.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100);

            RuleFor(x => x.Doctor.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20);

            RuleFor(x => x.Doctor.Specialization)
                .NotEmpty().WithMessage("Specialization is required.")
                .MaximumLength(100);

            RuleFor(x => x.Doctor.Qualification)
                .NotEmpty().WithMessage("Qualification is required.")
                .MaximumLength(100);

            RuleFor(x => x.Doctor.ExperienceYears)
                .GreaterThanOrEqualTo(0).WithMessage("Experience years cannot be negative.");

            RuleFor(x => x.Doctor.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100);
        }
    }
}
