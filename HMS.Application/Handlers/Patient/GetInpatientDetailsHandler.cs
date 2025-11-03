using AutoMapper;
using HMS.Application.DTO.Patient;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Patient;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Patient
{
    public class GetInpatientDetailsHandler : IRequestHandler<GetInpatientDetailsQuery, InpatientDetailsDto?>
    {
        private readonly IPatientVisitRepository _patientVisitRepository;
        private readonly IMapper _mapper;

        public GetInpatientDetailsHandler(IPatientVisitRepository patientVisitRepository, IMapper mapper)
        {
            _patientVisitRepository = patientVisitRepository;
            _mapper = mapper;
        }

        public async Task<InpatientDetailsDto?> Handle(GetInpatientDetailsQuery request, CancellationToken cancellationToken)
        {
            // Fetch the inpatient data from repository
            var inpatientEntity = await _patientVisitRepository.GetInpatientDetailsAsync(request.PatientId, request.VisitId);
            if (inpatientEntity == null)
                return null;

            // Map the entity or raw result to DTO
            var result = _mapper.Map<InpatientDetailsDto>(inpatientEntity);
            return result;
        }
    }
}
