namespace HMS.Application.Dto.Role
{
    public class EditRoleDto
    {
        public int Id { get; set; }          // Role ID (Primary Key)
        public string Name { get; set; }     // Role name
        public string Description { get; set; } // Optional role description
    }
}
