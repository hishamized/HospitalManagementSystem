using AutoMapper;
using HMS.Application.Commands.PatientVisits;
using HMS.Application.Dto;
using HMS.Application.DTOs;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.PatientVisits
{
    public class UpdatePatientVisitCommandHandler : IRequestHandler<UpdatePatientVisitCommand, bool>
    {
        private readonly IPatientVisitRepository _repository;
        private readonly IMapper _mapper;

        public UpdatePatientVisitCommandHandler(IPatientVisitRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdatePatientVisitCommand request, CancellationToken cancellationToken)
        {
            // Map command DTO → repository DTO (PatientVisitUpdateDto)
            var dto = _mapper.Map<PatientVisitUpdateDto>(request.PatientVisit);

            // Optional: set system fields like UpdatedAt if needed
            dto.UpdatedAt = DateTime.UtcNow;

            // Call repository (Dapper + Stored Procedure)
            var rowsAffected = await _repository.UpdateAsync(dto);

            // Return true if at least one row updated
            return rowsAffected > 0;
        }
    }
}
