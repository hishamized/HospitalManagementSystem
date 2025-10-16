using System.Collections.Generic;

namespace HMS.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., Admin, Doctor, Nurse
        public string Description { get; set; } = string.Empty;

        // Navigation property for many-to-many with users
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
