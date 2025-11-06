using HMS.Application.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Chat
{
    public record GetRecentChatsQuery : IRequest<List<RecentChatDto>>;
}
