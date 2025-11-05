using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Doctor
{
    public class AddDoctorRoundDto
    {
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public DateTime RoundDate { get; set; } = DateTime.UtcNow;
        public string? Observations { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescriptions { get; set; }
        public string? TestsRecommended { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? FollowUpInstructions { get; set; }
        public bool IsCritical { get; set; } = false;
    }
}
