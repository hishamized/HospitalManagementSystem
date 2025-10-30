using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Ward
{
    public class AssignDoctorWardDto
    {
        public int DoctorId { get; set; }
        public int WardId { get; set; }
        public string? Remarks { get; set; }

        // Optionally include the admin/user performing the action
        public int? AssignedBy { get; set; }
    }
}
