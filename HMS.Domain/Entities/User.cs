using System;
using System.Collections.Generic;

namespace HMS.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        // Personal info
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;

        // Security info
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property for many-to-many with roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
