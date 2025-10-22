using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }                          // Primary key
        public string DoctorCode { get; set; }              // Unique code for the doctor, e.g., DOC001
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}"; // Convenience property
        public string Gender { get; set; }                  // Male/Female/Other
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }          // e.g., Cardiology, Dermatology
        public string Qualification { get; set; }           // e.g., MBBS, MD, PhD
        public int ExperienceYears { get; set; }            // Total years of practice
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public bool IsActive { get; set; } = true;          // For soft delete / status
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }            // Nullable, updated when edited
    }

}
