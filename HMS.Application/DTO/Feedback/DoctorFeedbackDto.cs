using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Feedback
{
    public class DoctorFeedbackDto
    {
        public int FeedbackId { get; set; }
        public int DoctorId { get; set; }

        public string DoctorCode { get; set; }
        public string DoctorFullName { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }

        public int Rating { get; set; }
        public string Comments { get; set; }
        public string SubmittedFromIP { get; set; }
        public string SubmittedFromDevice { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsVisible { get; set; }
    }
}
