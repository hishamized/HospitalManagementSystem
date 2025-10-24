using HMS.Application.DTO.Appointment;
using MediatR;

namespace HMS.Application.Features.Appointments.Commands
{
    public class RescheduleAppointmentCommand : IRequest<bool>
    {
        public RescheduleAppointmentDto Appointment { get; set; }

        public RescheduleAppointmentCommand(RescheduleAppointmentDto appointment)
        {
            Appointment = appointment;
        }
    }
}
