using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.User
{
    public class VerifyOtpResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? NewPassword { get; set; }
    }
}
