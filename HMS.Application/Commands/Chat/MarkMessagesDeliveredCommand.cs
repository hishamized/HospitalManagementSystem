using MediatR;

namespace HMS.Application.Commands.Chat
{
    public class MarkMessagesDeliveredCommand : IRequest<int>
    {
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
    }
}