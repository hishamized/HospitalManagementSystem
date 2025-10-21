using System;

namespace HMS.Domain.Entities
{
    public class PatientVisit
    {
        public int Id { get; set; }

        // Foreign key to Patient
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        // Type of visit: "Inpatient" or "Outpatient"
        public string VisitType { get; set; } = string.Empty;

        // Common visit details
        public DateTime VisitDate { get; set; } = DateTime.UtcNow;
        public int? DoctorId { get; set; } // Optional FK to a Doctor entity if exists
        public string? DoctorName { get; set; } // Optional text if Doctor table not linked

        // Inpatient-specific details (nullable for outpatient visits)
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? RoomNumber { get; set; }

        // Treatment details (free text for now)
        public string? TreatmentDetails { get; set; }

        // Additional notes
        public string? Notes { get; set; }

        // System fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
