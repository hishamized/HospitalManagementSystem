using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.ViewModels.Ward
{
    public class DoctorWardAssignmentViewModel
    {
        public int Id { get; set; }

        // Doctor Info
        public int DoctorId { get; set; }
        public string DoctorCode { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }

        // Ward Info
        public int WardId { get; set; }
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public string WardType { get; set; }
        public string Location { get; set; }

        // Assignment Info
        public DateTime AssignedAt { get; set; }
        public int? AssignedBy { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        // Auditing
        public DateTime CreatedAt { get; set; }
    }
}
