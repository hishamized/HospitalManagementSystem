using AutoMapper;
using HMS.Application.Commands.Insurance;
using HMS.Application.DTO.Insurance;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Insurance
{
    public class AddInsuranceHandler : IRequestHandler<AddInsuranceCommand, int>
    {
        private readonly IInsuranceRepository _insuranceRepository;
        private readonly IMapper _mapper;

        public AddInsuranceHandler(IInsuranceRepository insuranceRepository, IMapper mapper)
        {
            _insuranceRepository = insuranceRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddInsuranceCommand request, CancellationToken cancellationToken)
        {
            // Map DTO → Entity
            var insuranceEntity = _mapper.Map<AddInsuranceDto>(request.Insurance);

            // Save to DB via repository
            var newId = await _insuranceRepository.AddAsync(request.Insurance);

            return newId;
        }
    }
}
