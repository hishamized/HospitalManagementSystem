using AutoMapper;
using HMS.Application.DTO.Allergy;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Allergy;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Allergy
{
    public class GetPatientAllergiesHandler : IRequestHandler<GetPatientAllergiesQuery, IEnumerable<ViewAllergyDto>>
    {
        private readonly IAllergyRepository _repository;
        private readonly IMapper _mapper;

        public GetPatientAllergiesHandler(IAllergyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAllergyDto>> Handle(GetPatientAllergiesQuery request, CancellationToken cancellationToken)
        {
            var allergies = await _repository.GetPatientAllergiesAsync(request.PatientId);
            return _mapper.Map<IEnumerable<ViewAllergyDto>>(allergies);
        }
    }
}
