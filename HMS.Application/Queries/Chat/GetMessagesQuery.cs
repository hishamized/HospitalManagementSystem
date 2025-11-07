using HMS.Application.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Chat
{
    public record GetMessagesQuery : IRequest<List<MessageDtoDetailed>>
    {
        public int ChatRoomId { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 50;
    }
}
