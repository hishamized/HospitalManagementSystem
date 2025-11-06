using HMS.Application.Commands.Chat;
using HMS.Application.DTO.Chat;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class StartChatHandler : IRequestHandler<StartChatCommand, RecentChatDto>
    {
        private readonly IChatRepository _repo;
        private readonly IUserContext _userContext;
        public StartChatHandler(IChatRepository repo, IUserContext userContext) { _repo = repo; _userContext = userContext; }

        public async Task<RecentChatDto> Handle(StartChatCommand req, CancellationToken ct)
        {
            var currentUserId = _userContext.UserId;
            var partnerId = req.UserId ?? await _repo.ResolveUserIdByIdentifier(req.Identifier);
            var chat = await _repo.GetOrCreateOneToOneChatAsync(currentUserId, partnerId);
            return await _repo.GetRecentChatDtoAsync(chat.Id, currentUserId);
        }
    }
}
