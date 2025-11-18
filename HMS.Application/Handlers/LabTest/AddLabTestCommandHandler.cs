using AutoMapper;
using HMS.Application.Commands.LabTest;
using HMS.Domain.Interfaces;
using MediatR;
using System.Threading;

namespace HMS.Application.Handlers.LabTest
{
    public class AddLabTestCommandHandler : IRequestHandler<AddLabTestCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddLabTestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<long> Handle(AddLabTestCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.LabRepository.AddLabTestAsync(request.dto, cancellationToken);
            return result;
        }
    }
}
