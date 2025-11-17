using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.PatientPortal
{
    public class GetPatientVisitsResponseDto
    {
        public int Id { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
