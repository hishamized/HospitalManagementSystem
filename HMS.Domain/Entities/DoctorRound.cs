using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class DoctorRound
    {
        public int Id { get; set; }                            // Primary Key

        // Foreign Keys
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public int WardId { get; set; }

        //Not a foreign key but important
        public int VisitId { get; set; }

        // Core Round Details
        public DateTime RoundDate { get; set; } = DateTime.UtcNow;   // Date and time of the round
        public string? Observations { get; set; }                     // Doctor’s notes or observations
        public string? Diagnosis { get; set; }                        // Updated diagnosis during the round
        public string? Prescriptions { get; set; }                    // Any new prescriptions or medication changes
        public string? TestsRecommended { get; set; }                 // Lab or imaging tests suggested
        public string? TreatmentPlan { get; set; }                    // Recommended treatment adjustments
        public string? FollowUpInstructions { get; set; }             // Any follow-up care notes
        public bool IsCritical { get; set; } = false;                 // Whether patient was in critical condition during round

        // Auditing and Status
        public bool IsActive { get; set; } = true;                    // For soft delete
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public Doctor Doctor { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public Ward Ward { get; set; } = null!;
    }

}
