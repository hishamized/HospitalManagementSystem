using System;

namespace HMS.Application.DTO.Patient
{
    public class UpdatePatientDto
    {
        // Primary Key
        public int Id { get; set; }

        // Basic information
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        // Contact and address details
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? AlternateContactNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }

        // Medical details
        public string? BloodGroup { get; set; }

        // Emergency contact
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? RelationshipWithEmergencyContact { get; set; }


        // System field
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
