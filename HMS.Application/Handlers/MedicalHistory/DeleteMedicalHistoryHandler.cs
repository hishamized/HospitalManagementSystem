using HMS.Application.Commands.MedicalHistory;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.MedicalHistory
{
    public class DeleteMedicalHistoryHandler : IRequestHandler<DeleteMedicalHistoryCommand, bool>
    {
        private readonly IMedicalHistoryRepository _repository;

        public DeleteMedicalHistoryHandler(IMedicalHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteMedicalHistoryCommand request, CancellationToken cancellationToken)
        {
            var deletedRows = await _repository.DeleteMedicalHistoryAsync(request.Id);
            return deletedRows > 0;
        }
    }
}
