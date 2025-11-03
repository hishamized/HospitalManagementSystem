using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Patient
{
    public class InpatientDetailsDto
    {
        // Patient Info
        public int PatientId { get; set; }
        public string PatientCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? AlternateContactNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? BloodGroup { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? RelationshipWithEmergencyContact { get; set; }

        // Visit Info
        public int VisitId { get; set; }
        public string VisitType { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public string? DoctorName { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? RoomNumber { get; set; }
        public string? TreatmentDetails { get; set; }
        public string? Notes { get; set; }

        // System Info
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
