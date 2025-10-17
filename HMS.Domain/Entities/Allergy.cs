using System;

namespace HMS.Domain.Entities
{
    public class Allergy
    {
        public int Id { get; set; }

        // Foreign key to Patient
        public int PatientId { get; set; }

        // Navigation property
        public Patient Patient { get; set; }

        // Allergy details
        public string Allergen { get; set; } = string.Empty;      // Name of the allergen
        public string Reaction { get; set; } = string.Empty;      // Reaction type/severity
        public string? Notes { get; set; }                        // Optional notes

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;               // Soft delete flag
    }
}
