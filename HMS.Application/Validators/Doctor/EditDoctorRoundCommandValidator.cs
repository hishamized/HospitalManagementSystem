using FluentValidation;
using HMS.Application.Commands.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Doctor
{
    public class EditDoctorRoundCommandValidator : AbstractValidator<EditDoctorRoundCommand>
    {
        public EditDoctorRoundCommandValidator()
        {
            // Ensure the DTO is not null
            RuleFor(x => x.DoctorRound).NotNull().WithMessage("Doctor round data is required.");

            // Validate individual DTO fields
            RuleFor(x => x.DoctorRound.Id)
                .GreaterThan(0).WithMessage("Invalid round ID.");

            RuleFor(x => x.DoctorRound.RoundDate)
                .NotEmpty().WithMessage("Round date is required.");

            RuleFor(x => x.DoctorRound.Observations)
                .MaximumLength(500).WithMessage("Observations cannot exceed 500 characters.");

            RuleFor(x => x.DoctorRound.Diagnosis)
                .MaximumLength(500).WithMessage("Diagnosis cannot exceed 500 characters.");

            RuleFor(x => x.DoctorRound.Prescriptions)
                .MaximumLength(500).WithMessage("Prescriptions cannot exceed 500 characters.");

            RuleFor(x => x.DoctorRound.TestsRecommended)
                .MaximumLength(500).WithMessage("Tests recommended cannot exceed 500 characters.");

            RuleFor(x => x.DoctorRound.TreatmentPlan)
                .MaximumLength(500).WithMessage("Treatment plan cannot exceed 500 characters.");

            RuleFor(x => x.DoctorRound.FollowUpInstructions)
                .MaximumLength(500).WithMessage("Follow-up instructions cannot exceed 500 characters.");

            // IsCritical is boolean, so optional validation
            RuleFor(x => x.DoctorRound.IsCritical)
                .NotNull().WithMessage("Critical status must be specified.");
        }
    }
}
