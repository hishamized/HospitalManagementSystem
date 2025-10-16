using HMS.Application.Commands;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using HMS.Application.DTO.Patient;

namespace HMS.Application.Handlers
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePatientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            // Map DTO → Entity
            var patient = _mapper.Map<CreatePatientDto>(request.Patient);

            // Assign system-generated fields
            patient.PatientCode = $"PT{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(100, 999)}";
            patient.CreatedAt = DateTime.UtcNow;
            patient.IsActive = true;

            // Save to database
            return await _unitOfWork.Patients.AddAsync(patient);
        }
    }
}
