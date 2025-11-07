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
    public class UpdateMessageStatusCommandHandler : IRequestHandler<UpdateMessageStatusCommand, Unit>
    {
        private readonly IChatRepository _chatRepository;

        public UpdateMessageStatusCommandHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<Unit> Handle(UpdateMessageStatusCommand request, CancellationToken cancellationToken)
        {
            // Call repository method to update status via stored procedure
            await _chatRepository.UpdateMessageStatusAsync(
                request.MessageId,
                request.UserId,
                request.IsDelivered,
                request.IsSeen
            );

            return Unit.Value;
        }
    }
}
