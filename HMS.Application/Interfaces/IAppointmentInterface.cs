using HMS.Application.DTO.Appointment;
using HMS.Application.ViewModel.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<int> AddAsync(AddAppointmentDTO appointmentDto);
        Task<IEnumerable<AppointmentViewModel>> GetAllAppointmentsAsync();
        Task<bool> RescheduleAppointmentAsync(RescheduleAppointmentDto appointment);

        Task<bool> DeleteAppointmentAsync(int appointmentId);
    }
}
