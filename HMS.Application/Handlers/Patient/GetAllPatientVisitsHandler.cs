using AutoMapper;
using HMS.Application.DTOs.PatientVisitDtos;
using HMS.Application.Features.PatientVisits.Queries;
using HMS.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Features.PatientVisits.Handlers
{
    public class GetAllPatientVisitsHandler : IRequestHandler<GetAllPatientVisitsQuery, IEnumerable<PatientVisitDto>>
    {
        private readonly IPatientVisitRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPatientVisitsHandler(IPatientVisitRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientVisitDto>> Handle(GetAllPatientVisitsQuery request, CancellationToken cancellationToken)
        {
            var visits = await _repository.GetAllPatientVisitsAsync();

            // If repository returns entities, you can map here; in our case, DTO is already returned
            return _mapper.Map<IEnumerable<PatientVisitDto>>(visits);
        }
    }
}
