using MediatR;

namespace HMS.Application.Features.Appointments.Commands
{
    // Command for deleting an appointment
    public class DeleteAppointmentCommand : IRequest<bool>
    {
        public int AppointmentId { get; set; }

        public DeleteAppointmentCommand(int appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}
