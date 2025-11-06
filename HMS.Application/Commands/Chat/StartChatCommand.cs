using HMS.Application.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Chat
{
    public record StartChatCommand : IRequest<RecentChatDto>
    {
        public int? UserId { get; init; }
        public string? Identifier { get; init; }
    }
}
