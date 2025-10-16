namespace HMS.Application.DTOs
{
    public class UserRoleDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Needed to verify
        public List<string> Roles { get; set; } = new List<string>();
    }
}
