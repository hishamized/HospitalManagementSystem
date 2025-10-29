using System;
using System.Collections.Generic;

namespace HMS.Domain.Entities
{
    public class Ward
    {
        public int Id { get; set; }                     // Primary Key
        public string WardCode { get; set; }            // Unique code e.g. "W001"
        public string WardName { get; set; }            // Human-readable name e.g. "General Ward"
        public string WardType { get; set; }            // e.g. "General", "Private", "ICU", "Emergency"
        public int Capacity { get; set; }               // Total number of beds
        public int OccupiedBeds { get; set; }           // Number of currently occupied beds
        public string Location { get; set; }            // Physical location or floor
        public string Description { get; set; }         // Optional details

        public bool IsActive { get; set; } = true;      // For soft-deactivation
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
