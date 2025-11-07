using HMS.Application.DTO.Chat;
using MediatR;
using System.Collections.Generic;

namespace HMS.Application.Queries.Chat
{
    public class GetUnseenMessagesQuery : IRequest<IEnumerable<MessageDto>>
    {
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
    }

}
