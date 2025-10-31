using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.User
{
    public class UserOtpResultDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string? NewPassword { get; set; }
        public string? FullName { get; internal set; }
    }

}
