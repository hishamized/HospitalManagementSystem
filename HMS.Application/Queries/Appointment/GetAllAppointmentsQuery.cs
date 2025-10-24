using HMS.Application.ViewModel.Appointment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Appointment
{
    public class GetAllAppointmentsQuery : IRequest<IEnumerable<AppointmentViewModel>>
    {
        // No properties needed for this simple "get all" query
    }
}
