using FluentValidation;
using HMS.Application.Features.Appointments.Commands;

namespace HMS.Application.Features.Appointments.Validators
{
    public class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
    {
        public RescheduleAppointmentCommandValidator()
        {
            RuleFor(x => x.Appointment.Id)
                .GreaterThan(0)
                .WithMessage("Appointment ID must be provided.");

            RuleFor(x => x.Appointment.DoctorId)
                .GreaterThan(0)
                .WithMessage("Doctor must be selected.");

            RuleFor(x => x.Appointment.AppointmentDate)
                .GreaterThan(DateTime.Now)
                .WithMessage("Appointment date must be in the future.");

            RuleFor(x => x.Appointment.Remarks)
                .MaximumLength(500)
                .WithMessage("Remarks cannot exceed 500 characters.");
        }
    }
}
