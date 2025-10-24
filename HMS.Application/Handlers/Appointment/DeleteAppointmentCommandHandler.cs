using HMS.Application.Interfaces;
using HMS.Application.Features.Appointments.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Features.Appointments.Handlers
{
    public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public DeleteAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<bool> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Call repository to delete the appointment
            var result = await _appointmentRepository.DeleteAppointmentAsync(request.AppointmentId);
            return result;
        }
    }
}
