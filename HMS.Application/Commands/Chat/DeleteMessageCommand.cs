using MediatR;

namespace HMS.Application.Commands.Chat
{
    public class DeleteMessageCommand : IRequest<Unit>
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
    }
}