using System;

namespace HMS.Application.Dto.Doctor
{
    public class EditDoctorDto
    {
        public int Id { get; set; }                          // Primary key
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }
        public string City { get; set; }

        // 🆕 Foreign Keys
        public int SlotId { get; set; }
        public int DepartmentId { get; set; }
    }
}
