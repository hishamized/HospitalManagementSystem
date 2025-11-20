using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.DoctorPortal
{
    public class FetchPatientsByDoctorDto
    {
        public int PatientId { get; set; }
        public string? PatientCode { get; set; } 
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? BloodGroup { get; set; }

    }
}
