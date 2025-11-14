using BCrypt.Net;
using HMS.Application.Dto;
using HMS.Application.DTO.PatientPortal;
using HMS.Application.Queries.PatientPortal;
using HMS.Domain.Interfaces;
using HMS.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.PatientPortal.Handlers
{
    public class PatientLoginQueryHandler : IRequestHandler<PatientLoginQuery, PatientLoginResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientLoginQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PatientLoginResultDto> Handle(PatientLoginQuery request, CancellationToken cancellationToken)
        {
            var patient = await _unitOfWork.PatientPortalRepository.LoginAsync(
                cancellationToken,
                request.LoginDto.Identifier
            );

            if (patient == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            // Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, patient.PasswordHash);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            // Clear hash before returning
            patient.PasswordHash = null;

            return patient;
        }
    }
}
