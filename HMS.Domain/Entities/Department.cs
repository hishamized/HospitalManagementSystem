// HMS.Domain/Entities/Department.cs
using System;
using System.Collections.Generic;

namespace HMS.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } = null!; // Department Name
        public string? Description { get; set; } // Optional description

        // Navigation Property: One Department can have many Doctors
        public virtual ICollection<Doctor>? Doctors { get; set; }
    }
}
