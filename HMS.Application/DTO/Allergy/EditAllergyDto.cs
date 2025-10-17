using System;

namespace HMS.Application.DTO.Allergy
{
    public class EditAllergyDto
    {
        public int Id { get; set; }                // allergy Id to edit
        public int PatientId { get; set; }         // FK to Patient
        public string Allergen { get; set; } = string.Empty;
        public string Reaction { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
