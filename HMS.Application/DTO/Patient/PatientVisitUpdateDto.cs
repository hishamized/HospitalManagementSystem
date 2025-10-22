using System;

namespace HMS.Application.Dto
{
    public class PatientVisitUpdateDto
    {
        public int Id { get; set; } // Mandatory, identifies the record

        public int PatientId { get; set; }
        public string VisitType { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }

        public int? DoctorId { get; set; }
        public string? DoctorName { get; set; }

        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? RoomNumber { get; set; }

        public string? TreatmentDetails { get; set; }
        public string? Notes { get; set; }

        // NEW: track last update
        public DateTime UpdatedAt { get; set; }
    }
}
