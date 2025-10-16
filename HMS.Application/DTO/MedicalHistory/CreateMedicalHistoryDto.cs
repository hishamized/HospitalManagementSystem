using System;

namespace HMS.Application.DTO.MedicalHistory
{
    public class CreateMedicalHistoryDto
    {
        // The Patient to which this history belongs
        public int PatientId { get; set; }

        // Medical details
        public string Condition { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }

        // Visit details
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public string? AttendingPhysician { get; set; }
    }
}
