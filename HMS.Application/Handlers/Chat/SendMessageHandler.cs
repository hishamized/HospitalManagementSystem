using HMS.Application.Commands.Chat;
using HMS.Application.DTO.Chat;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly IChatRepository _repo;
        private readonly IUserContext _userContext;
        private readonly IChatHubService _hubService;

        public SendMessageHandler(
            IChatRepository repo,
            IUserContext userContext,
            IChatHubService hubService)
        {
            _repo = repo;
            _userContext = userContext;
            _hubService = hubService;
        }

        public async Task<MessageDto> Handle(SendMessageCommand req, CancellationToken ct)
        {
            var msg = await _repo.AddMessageAsync(req.ChatRoomId, _userContext.UserId, req.Content);
            await _hubService.SendMessageToGroupAsync(req.ChatRoomId, msg);
            return msg;
        }
    }
}
