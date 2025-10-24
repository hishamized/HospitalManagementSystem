using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Appointment
{
    public class RescheduleAppointmentDto
    {
        public int Id { get; set; }  // Appointment ID

        public int DoctorId { get; set; } // New Doctor

        public DateTime AppointmentDate { get; set; } // New Date & Time

        public string? Remarks { get; set; } // Optional remarks
    }
}
