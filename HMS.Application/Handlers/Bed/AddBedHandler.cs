using AutoMapper;
using HMS.Application.Commands.Bed;
using HMS.Application.DTO.Bed;
using HMS.Application.Interfaces;
using MediatR;

namespace HMS.Application.Handlers.Bed
{
    public class AddBedHandler : IRequestHandler<AddBedCommand, int>
    {
        private readonly IMapper _mapper;
        private readonly IBedRepository _repository;

        public AddBedHandler(IMapper mapper, IBedRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<int> Handle(AddBedCommand request, CancellationToken cancellationToken)
        {
            var dto = _mapper.Map<AddBedDto>(request.Bed);

            var result = await _repository.CheckBedUniquenessAsync(dto.BedCode, dto.BedNumber);

            if (result == 1)
                throw new InvalidOperationException($"A bed with code '{dto.BedCode}' already exists.");
            else if (result == 2)
                throw new InvalidOperationException($"A bed with number '{dto.BedNumber}' already exists.");
            else if (result == 3)
                throw new InvalidOperationException($"Both bed code '{dto.BedCode}' and bed number '{dto.BedNumber}' already exist.");

            return await _repository.AddBedAsync(dto);
        }

    }
}
