using AutoMapper;
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
    public class EditBedHandler : IRequestHandler<EditBedCommand, int>
    {
        private readonly IMapper _mapper;
        private readonly IBedRepository _repository;

        public EditBedHandler(IMapper mapper, IBedRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<int> Handle(EditBedCommand request, CancellationToken cancellationToken)
        {
            var dto = _mapper.Map<EditBedDto>(request.Bed);

            var rowsAffected = await _repository.UpdateBedAsync(dto);

            return rowsAffected;
        }
    }
}
