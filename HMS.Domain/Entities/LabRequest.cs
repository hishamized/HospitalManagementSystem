using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class LabRequest
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = "Requested";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public Patient? Patient { get; set; } = null!;
        public Doctor? Doctor { get; set; } = null!;
    }
}
