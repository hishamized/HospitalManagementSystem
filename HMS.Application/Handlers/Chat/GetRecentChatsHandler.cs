using HMS.Application.DTO.Chat;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class GetRecentChatsHandler : IRequestHandler<GetRecentChatsQuery, List<RecentChatDto>>
    {
        private readonly IChatRepository _repo;
        private readonly IUserContext _u;
        public GetRecentChatsHandler(IChatRepository repo, IUserContext u) { _repo = repo; _u = u; }
        public Task<List<RecentChatDto>> Handle(GetRecentChatsQuery req, CancellationToken ct) => _repo.GetRecentChatsForUserAsync(_u.UserId);
    }
}
