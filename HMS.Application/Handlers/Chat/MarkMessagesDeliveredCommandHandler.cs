using HMS.Application.Commands.Chat;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class MarkMessagesDeliveredCommandHandler : IRequestHandler<MarkMessagesDeliveredCommand, int>
    {
        private readonly IChatRepository _chatRepository;

        public MarkMessagesDeliveredCommandHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<int> Handle(MarkMessagesDeliveredCommand request, CancellationToken cancellationToken)
        {
            return await _chatRepository.MarkMessagesAsDeliveredAsync(request.ChatRoomId, request.UserId, cancellationToken);
        }
    }
}