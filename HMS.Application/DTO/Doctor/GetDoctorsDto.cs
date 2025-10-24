using System;

namespace HMS.Application.Dto.Doctor
{
    public class GetDoctorsDto
    {
        public int Id { get; set; }
        public string DoctorCode { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; }

        // 🆕 Foreign Keys
        public int? SlotId { get; set; }
        public int? DepartmentId { get; set; }

        // 🆕 Optional navigation/display fields
        public string? SlotName { get; set; }

        public string? DaysOfWeek {get; set; }
        public string? DepartmentName { get; set; }
    }
}
