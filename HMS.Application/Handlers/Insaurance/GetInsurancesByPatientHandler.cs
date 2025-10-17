using AutoMapper;
using HMS.Application.DTO.Insurance;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Insurance;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Insurance
{
    public class GetInsurancesByPatientHandler : IRequestHandler<GetInsurancesByPatientQuery, IEnumerable<ViewInsuranceDto>>
    {
        private readonly IInsuranceRepository _insuranceRepository;
        private readonly IMapper _mapper;

        public GetInsurancesByPatientHandler(IInsuranceRepository insuranceRepository, IMapper mapper)
        {
            _insuranceRepository = insuranceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewInsuranceDto>> Handle(GetInsurancesByPatientQuery request, CancellationToken cancellationToken)
        {
            // Fetch all insurances from repository
            var insurances = await _insuranceRepository.GetByPatientIdAsync(request.PatientId);

            // Map to ViewInsuranceDto using AutoMapper
            var dtoList = _mapper.Map<IEnumerable<ViewInsuranceDto>>(insurances);

            return dtoList;
        }
    }
}
