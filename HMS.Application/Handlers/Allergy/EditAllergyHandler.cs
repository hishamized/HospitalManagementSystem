using AutoMapper;
using HMS.Application.Commands.Allergy;
using HMS.Application.DTO.Allergy;
using HMS.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Allergy
{
    public class EditAllergyHandler : IRequestHandler<EditAllergyCommand, bool>
    {
        private readonly IAllergyRepository _repository;
        private readonly IMapper _mapper;

        public EditAllergyHandler(IAllergyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(EditAllergyCommand request, CancellationToken cancellationToken)
        {
            // Map EditAllergyDto → Allergy entity
            var entity = _mapper.Map<EditAllergyDto>(request.Dto);

            // Call repository to update
            var updatedRows = await _repository.UpdateAllergyAsync(entity);

            // Return true if at least one row was affected
            return updatedRows;
        }
    }
}
