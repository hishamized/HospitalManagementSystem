using System;

namespace HMS.Application.DTO.Insurance
{
    public class ViewInsuranceDto
    {
        public int Id { get; set; } // Primary key
        public string ProviderName { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PatientId { get; set; }

        // Optional: include timestamps for display or editing
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
