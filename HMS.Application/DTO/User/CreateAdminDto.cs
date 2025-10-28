namespace HMS.Application.DTOs.Users
{
    public class CreateAdminDto
    {
        // Personal info
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;

        // Security info
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Plain-text temporarily, hash later in handler
        public bool IsActive { get; set; } = true;

        // Role mapping
        public int RoleId { get; set; }
    }
}
