using AutoMapper;
using HMS.Application.Commands.Ward;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Ward
{
    public class UpdateWardCommandHandler : IRequestHandler<UpdateWardCommand, int>
    {
        private readonly IWardRepository _wardRepository;
        private readonly IMapper _mapper;

        public UpdateWardCommandHandler(IWardRepository wardRepository, IMapper mapper)
        {
            _wardRepository = wardRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateWardCommand request, CancellationToken cancellationToken)
        {
            var ward = request.Ward;

            // Optionally map if you had a domain model conversion
            // var entity = _mapper.Map<Ward>(ward);

            var result = await _wardRepository.UpdateWardAsync(ward);

            return result; // Number of rows affected
        }
    }
}
