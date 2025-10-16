using AutoMapper;
using HMS.Application.DTO.Patient;
using HMS.Application.Queries;
using HMS.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public GetPatientByIdQueryHandler(IUnitOfWork unitOfWork, IPatientRepository patientRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _patientRepository = patientRepository;
            _mapper = mapper;

        }

        public async Task<PatientDto> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patientEntity = await _patientRepository.GetByIdAsync(request.Id);
            if (patientEntity == null) return null;

            return _mapper.Map<PatientDto>(patientEntity);
        }
    }
}
