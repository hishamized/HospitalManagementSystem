using HMS.Application.DTO.Chat;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Chat;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class GetUnseenMessagesQueryHandler : IRequestHandler<GetUnseenMessagesQuery, IEnumerable<MessageDto>>
    {
        private readonly IChatRepository _chatRepository;

        public GetUnseenMessagesQueryHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<IEnumerable<MessageDto>> Handle(GetUnseenMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _chatRepository.GetUnseenMessagesAsync(request.ChatRoomId, request.UserId, cancellationToken);
        }
    }
}