using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.ViewModel.Appointment
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        // Patient Info
        public int PatientId { get; set; }
        public string PatientCode { get; set; }
        public string PatientFullName { get; set; }

        // Doctor Info
        public int DoctorId { get; set; }
        public string DoctorCode { get; set; }
        public string DoctorFullName { get; set; }

        // Appointment Info
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string? Remarks { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
