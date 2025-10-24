using AutoMapper;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Appointment;
using HMS.Application.ViewModel.Appointment;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Appointment
{
    public class GetAllAppointmentsHandler : IRequestHandler<GetAllAppointmentsQuery, IEnumerable<AppointmentViewModel>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public GetAllAppointmentsHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentViewModel>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointmentsData = await _appointmentRepository.GetAllAppointmentsAsync();

            // Map data if needed (though already returns AppointmentViewModel)
            var mappedAppointments = _mapper.Map<IEnumerable<AppointmentViewModel>>(appointmentsData);

            return mappedAppointments;
        }
    }
}
