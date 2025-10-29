using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Ward
{
    public class WardDto
    {
        public int Id { get; set; }                     // Primary Key
        public string WardCode { get; set; }            // Unique code e.g. "W001"
        public string WardName { get; set; }            // Human-readable name e.g. "General Ward"
        public string WardType { get; set; }            // e.g. "General", "Private", "ICU", "Emergency"
        public int Capacity { get; set; }               // Total number of beds
        public int OccupiedBeds { get; set; }           // Number of currently occupied beds
        public string Location { get; set; }            // Physical location or floor
        public string Description { get; set; }         // Optional details
        public bool IsActive { get; set; }              // For soft-deactivation
        public DateTime CreatedAt { get; set; }         // Creation timestamp
        public DateTime? UpdatedAt { get; set; }        // Last update timestamp
    }
}
