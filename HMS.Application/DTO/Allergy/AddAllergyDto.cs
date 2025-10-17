using System;

namespace HMS.Application.DTO.Allergy
{
    public class AddAllergyDto
    {
        public int PatientId { get; set; }       // FK to Patient
        public string Allergen { get; set; } = string.Empty;
        public string Reaction { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
