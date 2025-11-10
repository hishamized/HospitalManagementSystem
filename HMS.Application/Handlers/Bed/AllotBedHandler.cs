using AutoMapper;
using HMS.Application.Commands.Bed;
using HMS.Application.DTO.Bed;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bed
{
    public class AllotBedHandler : IRequestHandler<AllotBedCommand, int>
    {
        private readonly IBedRepository _bedRepository;
        private readonly IMapper _mapper;

        public AllotBedHandler(IBedRepository bedRepository, IMapper mapper)
        {
            _bedRepository = bedRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AllotBedCommand request, CancellationToken cancellationToken)
        {
            // Map the command DTO to repository DTO
            var entity = _mapper.Map<AllotBedDto>(request.Dto);

            // Call repository to allot bed and return the new Bed ID
            var newBedId = await _bedRepository.AllotBedAsync(entity);

            return newBedId;
        }
    }
}
