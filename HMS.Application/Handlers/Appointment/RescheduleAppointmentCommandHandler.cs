using AutoMapper;
using HMS.Application.DTO.Appointment;
using HMS.Application.Features.Appointments.Commands;
using HMS.Application.Interfaces;
using MediatR;

namespace HMS.Application.Features.Appointments.Handlers
{
    public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public RescheduleAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Map command DTO to repository DTO (optional if same)
            var dto = _mapper.Map<RescheduleAppointmentDto>(request.Appointment);

            // Call repository to reschedule
            var result = await _appointmentRepository.RescheduleAppointmentAsync(dto);

            return result;
        }
    }
}
