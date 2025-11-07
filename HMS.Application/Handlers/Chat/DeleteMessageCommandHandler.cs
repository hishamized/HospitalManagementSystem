using HMS.Application.Commands.Chat;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Unit>
    {
        private readonly IChatRepository _chatRepository;

        public DeleteMessageCommandHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<Unit> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            await _chatRepository.DeleteMessageAsync(request.MessageId, request.UserId, cancellationToken);
            return Unit.Value;
        }
    }
}