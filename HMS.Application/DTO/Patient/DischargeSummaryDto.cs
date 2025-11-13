using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Patient
{
    public class DischargeSummaryDto
    {
        // Core Visit Info
        public int VisitId { get; set; }
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; } = string.Empty;

        // Admission & Discharge Details
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? RoomNumber { get; set; }

        // Doctor Info
        public int? DoctorId { get; set; }
        public string? DoctorName { get; set; }

        // Treatment Summary
        public string? TreatmentDetails { get; set; }
        public string? Notes { get; set; }

        // Patient Info (flattened from Patient table)
        public int PatientId { get; set; }
        public string PatientCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string? Address { get; set; }

        // Bed & Ward Info (from PatientBedWards)
        public int? WardId { get; set; }
        public string? WardName { get; set; }
        public int? BedId { get; set; }
        public string? BedNumber { get; set; }

        public DateTime? BedAssignedAt { get; set; }
        public DateTime? BedReleasedAt { get; set; }

        // List of doctor rounds
        public List<DoctorRoundDto> DoctorRounds { get; set; } = new();
    }
}
