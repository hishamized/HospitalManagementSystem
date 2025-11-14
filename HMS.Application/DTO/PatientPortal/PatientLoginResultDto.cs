using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.PatientPortal
{
    public class PatientLoginResultDto
    {
        public int PatientId { get; set; }
        public string PatientCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public int RoleId { get; set; }

        public string PasswordHash { get; set; } = string.Empty;
    }
}
