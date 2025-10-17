using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        // Unique hospital-assigned patient code
        public string PatientCode { get; set; } = string.Empty;

        // Basic information
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        // Contact and identity details
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? AlternateContactNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }

        // Medical details
        public string? BloodGroup { get; set; }

        // Emergency contact details
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? RelationshipWithEmergencyContact { get; set; }

        // System details
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<MedicalHistory>? MedicalHistories { get; set; }
        public ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();
        public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();


    }
}
