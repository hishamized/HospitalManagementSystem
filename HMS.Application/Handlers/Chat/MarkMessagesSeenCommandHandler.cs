using HMS.Application.Commands.Chat;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class MarkMessagesSeenCommandHandler : IRequestHandler<MarkMessagesSeenCommand, int>
    {
        private readonly IChatRepository _chatRepository;

        public MarkMessagesSeenCommandHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<int> Handle(MarkMessagesSeenCommand request, CancellationToken cancellationToken)
        {
            return await _chatRepository.MarkMessagesSeenAsync(request.UserId, request.MessageIds);
        }
    }
}
