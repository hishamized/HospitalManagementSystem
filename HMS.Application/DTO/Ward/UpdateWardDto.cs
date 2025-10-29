using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Ward
{
    public class UpdateWardDto
    {
        public int Id { get; set; }                     // Primary Key (for identification)
        public string WardCode { get; set; }            // Unique code (e.g. "W001")
        public string WardName { get; set; }            // Human-readable name
        public string WardType { get; set; }            // Type (e.g. "General", "ICU", etc.)
        public int Capacity { get; set; }               // Total number of beds
        public int OccupiedBeds { get; set; }           // Currently occupied beds
        public string Location { get; set; }            // Location or floor
        public string Description { get; set; }         // Optional details
        public bool IsActive { get; set; } = true;      // Active flag
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
