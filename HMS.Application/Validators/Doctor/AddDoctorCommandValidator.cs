using FluentValidation;
using HMS.Application.Features.Doctors.Commands;
using System;

namespace HMS.Application.Validators.Doctor
{
    public class AddDoctorCommandValidator : AbstractValidator<AddDoctorCommand>
    {
        public AddDoctorCommandValidator()
        {
            RuleFor(x => x.Doctor.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.Doctor.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.Doctor.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(g => g == "Male" || g == "Female" || g == "Other")
                .WithMessage("Gender must be Male, Female, or Other.");

            RuleFor(x => x.Doctor.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.Doctor.Email)
                .EmailAddress().When(d => !string.IsNullOrEmpty(d.Doctor.Email))
                .WithMessage("Invalid email address.");

            RuleFor(x => x.Doctor.PhoneNumber)
                .Matches(@"^\+?\d{7,15}$").When(d => !string.IsNullOrEmpty(d.Doctor.PhoneNumber))
                .WithMessage("Invalid phone number.");

            RuleFor(x => x.Doctor.Specialization)
                .MaximumLength(50).WithMessage("Specialization cannot exceed 50 characters.");

            RuleFor(x => x.Doctor.Qualification)
                .MaximumLength(50).WithMessage("Qualification cannot exceed 50 characters.");

            RuleFor(x => x.Doctor.ExperienceYears)
                .GreaterThanOrEqualTo(0).WithMessage("Experience years must be zero or positive.");

            RuleFor(x => x.Doctor.Address)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

            RuleFor(x => x.Doctor.City)
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");

            RuleFor(x => x.Doctor.State)
                .MaximumLength(50).WithMessage("State cannot exceed 50 characters.");

            RuleFor(x => x.Doctor.ZipCode)
                .MaximumLength(20).WithMessage("Zip code cannot exceed 20 characters.");
        }
    }
}
