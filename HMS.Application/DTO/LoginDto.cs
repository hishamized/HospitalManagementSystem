using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTOs
{
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; } = string.Empty; // combine username/email
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }
}
