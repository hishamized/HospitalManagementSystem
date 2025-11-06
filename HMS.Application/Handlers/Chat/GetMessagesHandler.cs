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
    public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
    {
        private readonly IChatRepository _repo;
        public GetMessagesHandler(IChatRepository repo) { _repo = repo; }
        public Task<List<MessageDto>> Handle(GetMessagesQuery req, CancellationToken ct) => _repo.GetMessagesAsync(req.ChatRoomId, req.Page, req.PageSize);
    }

}
