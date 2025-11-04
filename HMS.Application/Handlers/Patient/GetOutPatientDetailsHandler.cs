using AutoMapper;
using HMS.Application.DTO.Patient;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Patient;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Patient
{
    public class GetOutPatientDetailsHandler : IRequestHandler<GetOutPatientDetailsQuery, OutPatientDetailsDto?>
    {
        private readonly IPatientVisitRepository _repository;
        private readonly IMapper _mapper;

        public GetOutPatientDetailsHandler(IPatientVisitRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OutPatientDetailsDto?> Handle(GetOutPatientDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetOutPatientDetailsAsync(request.PatientId, request.VisitId);
            return result;
        }
    }
}
