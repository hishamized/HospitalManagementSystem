using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.DoctorPortal
{
    public class LabRequestCreateDto
    {
        public int PatientId { get; set; }
        public int? DoctorId { get; set; } // controller will fill from claims
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

}
