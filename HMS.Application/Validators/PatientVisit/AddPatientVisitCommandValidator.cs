using FluentValidation;
using HMS.Application.Features.PatientVisits.Commands;
using System;

namespace HMS.Application.Validators.PatientVisit
{
    public class AddPatientVisitCommandValidator : AbstractValidator<AddPatientVisitCommand>
    {
        public AddPatientVisitCommandValidator()
        {
            RuleFor(x => x.PatientVisit.PatientId)
                .GreaterThan(0)
                .WithMessage("Patient must be selected.");

            RuleFor(x => x.PatientVisit.VisitType)
                .NotEmpty()
                .WithMessage("Visit type is required.");

            RuleFor(x => x.PatientVisit.VisitDate)
                .NotEmpty()
                .WithMessage("Visit date is required.")
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Visit date cannot be in the future.");

            RuleFor(x => x.PatientVisit.DoctorName)
                .MaximumLength(100)
                .WithMessage("Doctor name cannot exceed 100 characters.");

            RuleFor(x => x.PatientVisit.AdmissionDate)
                .LessThanOrEqualTo(x => x.PatientVisit.DischargeDate ?? DateTime.MaxValue)
                .When(x => x.PatientVisit.AdmissionDate.HasValue && x.PatientVisit.DischargeDate.HasValue)
                .WithMessage("Admission date must be before discharge date.");

            RuleFor(x => x.PatientVisit.RoomNumber)
                .MaximumLength(50)
                .WithMessage("Room number cannot exceed 50 characters.");

            RuleFor(x => x.PatientVisit.TreatmentDetails)
                .MaximumLength(1000)
                .WithMessage("Treatment details cannot exceed 1000 characters.");

            RuleFor(x => x.PatientVisit.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters.");
        }
    }
}
