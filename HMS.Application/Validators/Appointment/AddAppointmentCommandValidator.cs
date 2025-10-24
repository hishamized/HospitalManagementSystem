using FluentValidation;
using HMS.Application.Commands.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Validators.Appointment
{
    public class AddAppointmentCommandValidator : AbstractValidator<AddAppointmentCommand>
    {
        public AddAppointmentCommandValidator()
        {
            RuleFor(x => x.AppointmentDto.PatientId)
                .GreaterThan(0).WithMessage("Patient is required.");

            RuleFor(x => x.AppointmentDto.DoctorId)
                .GreaterThan(0).WithMessage("Doctor is required.");

            RuleFor(x => x.AppointmentDto.AppointmentDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");

            RuleFor(x => x.AppointmentDto.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(50);

            RuleFor(x => x.AppointmentDto.Remarks)
                .MaximumLength(500).WithMessage("Remarks cannot exceed 500 characters.");

        }
    }
}
