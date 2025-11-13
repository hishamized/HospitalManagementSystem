using AutoMapper;
using HMS.Application.DTO.Patient;
using HMS.Application.Queries.Patient;
using HMS.Domain.Interfaces; 
using MediatR;

namespace HMS.Application.Handlers.Patient
{
    public class GetDischargeSummaryQueryHandler
    : IRequestHandler<GetDischargeSummaryQuery, DischargeSummaryDto>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public GetDischargeSummaryQueryHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<DischargeSummaryDto> Handle(GetDischargeSummaryQuery request, CancellationToken cancellationToken)
        {
            // Call Repository to fetch data using stored procedure
            var dischargeSummary = await _patientRepository.GetDischargeSummaryAsync(request.VisitId);

            if (dischargeSummary == null)
                throw new Exception($"No discharge summary found for VisitId {request.VisitId}");

            // If mapping needed (not always necessary since repository already maps to DTO)
            return _mapper.Map<DischargeSummaryDto>(dischargeSummary);
        }
    }
}
