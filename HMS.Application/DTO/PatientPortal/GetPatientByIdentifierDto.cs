using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.PatientPortal
{
    public class GetPatientByIdentifierDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PatientCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

