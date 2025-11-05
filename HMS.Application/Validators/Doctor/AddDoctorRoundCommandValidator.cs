using FluentValidation;
using HMS.Application.Commands.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Doctor
{
    public class AddDoctorRoundCommandValidator : AbstractValidator<AddDoctorRoundCommand>
    {
        public AddDoctorRoundCommandValidator()
        {
            RuleFor(x => x.DoctorRound)
                .NotNull().WithMessage("Doctor round data is required.");

            RuleFor(x => x.DoctorRound.PatientId)
                .GreaterThan(0).WithMessage("Valid PatientId is required.");

            RuleFor(x => x.DoctorRound.DoctorId)
                .GreaterThan(0).WithMessage("Valid DoctorId is required.");

            RuleFor(x => x.DoctorRound.RoundDate)
                .LessThanOrEqualTo(System.DateTime.UtcNow)
                .WithMessage("Round date cannot be in the future.");

            RuleFor(x => x.DoctorRound.Observations)
                .MaximumLength(2000);

            RuleFor(x => x.DoctorRound.Diagnosis)
                .MaximumLength(2000);

            RuleFor(x => x.DoctorRound.Prescriptions)
                .MaximumLength(2000);

            RuleFor(x => x.DoctorRound.TestsRecommended)
                .MaximumLength(2000);

            RuleFor(x => x.DoctorRound.TreatmentPlan)
                .MaximumLength(2000);

            RuleFor(x => x.DoctorRound.FollowUpInstructions)
                .MaximumLength(2000);
        }
    }
}
