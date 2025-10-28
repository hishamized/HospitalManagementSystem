using MediatR;

namespace HMS.Application.Commands.Role
{
    // The request returns a tuple with success flag, message, and rows affected count
    public class TheDeleteRoleCommand : IRequest<(bool Success, string Message, int RowsAffected)>
    {
        public int RoleId { get; }

        public TheDeleteRoleCommand(int roleId)
        {
            RoleId = roleId;
        }
    }
}
