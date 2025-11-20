using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.DoctorPortal
{
    public class DoctorLoginResultDto
    {
        public int DoctorId { get; set; }
        public string DoctorCode { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
    }
}
