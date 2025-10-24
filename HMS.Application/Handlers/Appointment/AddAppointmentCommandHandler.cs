using HMS.Application.Commands.Appointment;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Appointment
{
    public class AddAppointmentCommandHandler : IRequestHandler<AddAppointmentCommand, int>
    {
        private readonly Interfaces.IAppointmentRepository _appointmentRepository;

        public AddAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<int> Handle(AddAppointmentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.AppointmentDto;

            // Call repository to add appointment
            var newAppointmentId = await _appointmentRepository.AddAsync(dto);

            return newAppointmentId;
        }
    }
}
