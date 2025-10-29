using AutoMapper;
using HMS.Application.Commands.Ward;
using HMS.Application.DTO.Ward;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Ward
{
    public class CreateWardHandler : IRequestHandler<CreateWardCommand, int>
    {
        private readonly IMapper _mapper;
        private readonly IWardRepository _wardRepository;

        public CreateWardHandler(IMapper mapper, IWardRepository wardRepository)
        {
            _mapper = mapper;
            _wardRepository = wardRepository;
        }

        public async Task<int> Handle(CreateWardCommand request, CancellationToken cancellationToken)
        {
            // Map DTO if needed — here we already have DTO, but AutoMapper keeps consistency
            var wardDto = _mapper.Map<CreateWardDto>(request.Ward);

            // Call repository method that runs stored procedure
            var newWardId = await _wardRepository.AddWardAsync(wardDto);

            return newWardId;
        }
    }
}
