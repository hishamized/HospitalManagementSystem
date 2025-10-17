using System;

namespace HMS.Application.DTO.MedicalHistory
{
    public class EditMedicalHistoryDto
    {
        public int Id { get; set; }

        // Editable medical details
        public string Condition { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }

        // Editable visit details
        public DateTime RecordDate { get; set; }
        public string? AttendingPhysician { get; set; }
    }
}
