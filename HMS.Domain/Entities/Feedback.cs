using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace HMS.Domain.Entities
{
    public class Feedback
    {
        public int Id { get; set; }                                // Primary key

        public int DoctorId { get; set; }                          // Foreign key to Doctor
        public Doctor Doctor { get; set; }                         // Navigation property

        public int Rating { get; set; }                            // 1 to 5 stars
        public string Comments { get; set; }                       // Patient's written feedback (optional)

        public string SubmittedFromIP { get; set; }                // For audit/security (anonymous tracking)
        public string SubmittedFromDevice { get; set; }            // Optional (browser, mobile, etc.)

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow; // Timestamp of feedback submission

        public bool IsVisible { get; set; } = true;                // For moderation or hiding inappropriate feedback
    }
}
