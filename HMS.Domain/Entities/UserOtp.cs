using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class UserOtp
    {
        public int Id { get; set; }

        // Foreign key to User
        public int UserId { get; set; }

        // The actual OTP code sent to the user
        public string OtpCode { get; set; } = string.Empty;

        // Time until OTP is valid (e.g., 5 or 10 minutes)
        public DateTime ExpiryAt { get; set; }

        // Whether the OTP has been successfully used
        public bool IsUsed { get; set; } = false;

        // Timestamps for auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property (optional, for EF or mapping)
        public User? User { get; set; }
    }
}
