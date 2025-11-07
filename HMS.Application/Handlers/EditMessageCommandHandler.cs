using HMS.Application.Commands.Chat;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, Unit>
    {
        private readonly IChatRepository _chatRepository;

        public EditMessageCommandHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<Unit> Handle(EditMessageCommand request, CancellationToken cancellationToken)
        {
            await _chatRepository.EditMessageAsync(request.MessageId, request.NewContent, request.UserId, cancellationToken);
            return Unit.Value;
        }
    }
}