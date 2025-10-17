using AutoMapper;
using HMS.Application.DTO.MedicalHistory;
using HMS.Application.Interfaces;
using HMS.Application.Queries.MedicalHistory;
using HMS.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.MedicalHistory
{
    public class GetPatientMedicalHistoryHandler
        : IRequestHandler<GetPatientMedicalHistoryQuery, IEnumerable<GetPatientMedicalHistoryDto>>
    {
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;
        private readonly IMapper _mapper;

        public GetPatientMedicalHistoryHandler(
            IMedicalHistoryRepository medicalHistoryRepository,
            IMapper mapper)
        {
            _medicalHistoryRepository = medicalHistoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetPatientMedicalHistoryDto>> Handle(
            GetPatientMedicalHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var medicalHistoryEntities = await _medicalHistoryRepository.GetPatientMedicalHistoryAsync(request.PatientId);

            return _mapper.Map<IEnumerable<GetPatientMedicalHistoryDto>>(medicalHistoryEntities);
        }
    }
}
