using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        // Foreign Keys
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        // Navigation properties
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }

        // Appointment details
        public DateTime AppointmentDate { get; set; }  // date + time
        public string Status { get; set; }             // e.g., Scheduled, Cancelled, Completed
        public string Remarks { get; set; }            // optional notes

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
