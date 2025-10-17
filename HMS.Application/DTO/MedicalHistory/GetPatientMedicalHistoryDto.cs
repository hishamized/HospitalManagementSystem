using System;

namespace HMS.Application.DTO.MedicalHistory
{
    public class GetPatientMedicalHistoryDto
    {
        public int Id { get; set; }

        // Basic medical details
        public string Condition { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }

        // Visit details
        public DateTime RecordDate { get; set; }
        public string? AttendingPhysician { get; set; }

        // Audit information
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
