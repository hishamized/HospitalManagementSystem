using AutoMapper;
using HMS.Application.DTOs.PatientVisitDtos;
using HMS.Application.Features.PatientVisits.Commands;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Features.PatientVisits.Handlers
{
    public class AddPatientVisitHandler : IRequestHandler<AddPatientVisitCommand, int>
    {
        private readonly IInsuranceRepository _insuranceRepository; // 👈 we will replace this with IPatientVisitRepository
        private readonly IMapper _mapper;

        public AddPatientVisitHandler(IPatientVisitRepository patientVisitRepository, IMapper mapper)
        {
            _mapper = mapper;
            _patientVisitRepository = patientVisitRepository;
        }

        private readonly IPatientVisitRepository _patientVisitRepository;

        public async Task<int> Handle(AddPatientVisitCommand request, CancellationToken cancellationToken)
        {
            // Map DTO to entity using AutoMapper
            var patientVisitEntity = _mapper.Map<PatientVisit>(request.PatientVisit);

            // Call repository (which internally executes stored procedure)
            var newVisitId = await _patientVisitRepository.AddPatientVisitAsync(request.PatientVisit);

            return newVisitId;
        }
    }
}
