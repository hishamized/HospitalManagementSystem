using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class PatientBedWard
    {
        public int Id { get; set; }

        // Foreign keys
        public int PatientId { get; set; }
        public int WardId { get; set; }
        public int BedId { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Ward Ward { get; set; } = null!;
        public Bed Bed { get; set; } = null!;

        // Admission details
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReleasedAt { get; set; }

        // Status info
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

}
