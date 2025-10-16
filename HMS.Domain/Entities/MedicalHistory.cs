using System;

namespace HMS.Domain.Entities
{
    public class MedicalHistory
    {
        public int Id { get; set; }

        // Foreign key to Patient
        public int PatientId { get; set; }

        // Navigation property to Patient
        public Patient Patient { get; set; }

        // Medical details
        public string Condition { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }

        // Visit details
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public string? AttendingPhysician { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
