using HMS.Application.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Chat
{
    public record SendMessageCommand : IRequest<MessageDto>
    {
        public int ChatRoomId { get; init; }
        public string Content { get; init; } = string.Empty;
    }
}
