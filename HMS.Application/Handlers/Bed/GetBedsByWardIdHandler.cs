using HMS.Application.Commands.Bed;
using HMS.Application.DTO.Bed;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bed
{
    public class GetBedsByWardIdHandler : IRequestHandler<GetBedsByWardIdCommand, IEnumerable<BedDropdownDto>>
    {
        private readonly IBedRepository _bedRepository;

        public GetBedsByWardIdHandler(IBedRepository bedRepository)
        {
            _bedRepository = bedRepository;
        }

        public async Task<IEnumerable<BedDropdownDto>> Handle(GetBedsByWardIdCommand request, CancellationToken cancellationToken)
        {
            return await _bedRepository.GetBedsByWardIdAsync(request.WardId);
        }
    }
}
