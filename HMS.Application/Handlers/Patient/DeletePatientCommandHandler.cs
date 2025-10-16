using HMS.Application.Commands;
using HMS.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers
{
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePatientCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _unitOfWork.Patients.DeleteAsync(request.Id);
            return deleted > 0;
        }
    }
}
