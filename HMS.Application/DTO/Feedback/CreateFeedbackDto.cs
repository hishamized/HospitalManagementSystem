using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Feedback
{
    public class CreateFeedbackDto
    {
        public int DoctorId { get; set; }              // Selected doctor from dropdown
        public int Rating { get; set; }                // 1 to 5 star rating
        public string? Comments { get; set; }          // Optional patient comment
        public string SubmittedFromIP { get; set; }    // Captured from request
        public string SubmittedFromDevice { get; set; }// Optional device/browser info
    }
}
