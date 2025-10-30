using System;

namespace HMS.Domain.Entities
{
    public class DoctorWard
    {
        public int Id { get; set; }

        // Foreign Keys
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int WardId { get; set; }
        public Ward Ward { get; set; } = null!;

        // Assignment Info
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;  // When the doctor was assigned
        public int? AssignedBy { get; set; }                         // User/Admin who assigned

        public DateTime? UnassignedAt { get; set; }                  // When the doctor was unassigned
        public int? UnassignedBy { get; set; }                       // User/Admin who unassigned

        public bool IsActive { get; set; } = true;                   // Active = assigned, false = unassigned
        public string? Remarks { get; set; }                         // Optional notes or reasons

        // Standard Timestamps for auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
