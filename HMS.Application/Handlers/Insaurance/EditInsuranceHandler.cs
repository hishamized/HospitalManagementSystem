using AutoMapper;
using HMS.Application.Commands.Insurance;
using HMS.Application.DTO.Insurance;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Insurance
{
    public class EditInsuranceHandler : IRequestHandler<EditInsuranceCommand, bool>
    {
        private readonly IInsuranceRepository _repository;
        private readonly IMapper _mapper;

        public EditInsuranceHandler(IInsuranceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(EditInsuranceCommand request, CancellationToken cancellationToken)
        {
            var updated = await _repository.UpdateInsuranceAsync(request.Dto);
            // Return true if update succeeded
            return updated;
        }
    }
}
