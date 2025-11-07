using MediatR;

namespace HMS.Application.Commands.Chat
{
    public class EditMessageCommand : IRequest<Unit>
    {
        public int MessageId { get; set; }
        public string NewContent { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}