using System;

namespace HMS.Application.DTO.Allergy
{
    public class ViewAllergyDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Allergen { get; set; } = string.Empty;
        public string Reaction { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
