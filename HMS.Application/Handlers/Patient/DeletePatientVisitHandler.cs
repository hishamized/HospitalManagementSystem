using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Commands.PatientVisit
{
    public class DeletePatientVisitHandler : IRequestHandler<DeletePatientVisitCommand, bool>
    {
        private readonly IPatientVisitRepository _repository;

        public DeletePatientVisitHandler(IPatientVisitRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeletePatientVisitCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteAsync(request.Id);
        }
    }
}
