using System;

namespace HMS.Application.DTO.Feedback
{
    public class FeedbackListDto
    {
        public int Id { get; set; }                     // Feedback ID
        public string DoctorName { get; set; }          // Doctor's full name (from CONCAT)
        public string DepartmentName { get; set; }      // Department name (from dept.Name)
        public int Rating { get; set; }                 // Rating (1–5)
        public string Comments { get; set; }            // Feedback text/comment
        public string SubmittedFromIP { get; set; }     // IP address of submitter
        public string SubmittedFromDevice { get; set; } // Device info (browser, OS, etc.)
        public DateTime SubmittedAt { get; set; }       // Feedback submission timestamp
        public bool IsVisible { get; set; }             // Visibility flag (filtered in SP)
    }
}
