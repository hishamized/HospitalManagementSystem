using AutoMapper;
using HMS.Application.Commands.Bed;
using HMS.Application.DTO.Bed;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bed
{
    public class AllotBedHandler : IRequestHandler<AllotBedCommand, bool>
    {
        private readonly IBedRepository _bedRepository;
        private readonly IMapper _mapper;

        public AllotBedHandler(IBedRepository bedRepository, IMapper mapper)
        {
            _bedRepository = bedRepository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(AllotBedCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<AllotBedDto>(request.Dto);
            return await _bedRepository.AllotBedAsync(entity);
        }
    }
}
