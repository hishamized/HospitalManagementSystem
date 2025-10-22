using System;

namespace HMS.Application.DTOs.PatientVisitDtos
{
    public class AddPatientVisitDto
    {
        public int PatientId { get; set; }
        public string VisitType { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; } = DateTime.UtcNow;
        public int? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? RoomNumber { get; set; }
        public string? TreatmentDetails { get; set; }
        public string? Notes { get; set; }
    }
}
