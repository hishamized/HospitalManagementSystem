// HMS.Domain/Entities/Insurance.cs
using System;

namespace HMS.Domain.Entities
{
    public class Insurance
    {
        public int Id { get; set; } // Primary key
        public string ProviderName { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Foreign key
        public int PatientId { get; set; }

        // Navigation property
        public virtual Patient Patient { get; set; } = null!;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
