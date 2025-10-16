using HMS.Application.Commands;
using HMS.Domain.Interfaces;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using HMS.Application.DTO.Patient;

namespace HMS.Application.Handlers
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdatePatientCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            // Map DTO → UpdatePatientDto (repository expects DTO)
            var patientDto = _mapper.Map<UpdatePatientDto>(request.Patient);

            // Set system field
            patientDto.UpdatedAt = DateTime.UtcNow;

            // Call repository
            var updated = await _unitOfWork.Patients.UpdateAsync(patientDto);

            return updated > 0;
        }
    }
}
