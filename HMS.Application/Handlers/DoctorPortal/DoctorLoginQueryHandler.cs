using HMS.Application.DTO.DoctorPortal;
using HMS.Application.Queries.DoctorPortal;
using HMS.Application.Queries.PatientPortal;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorPortal
{
    public class DoctorLoginQueryHandler : IRequestHandler<DoctorLoginQuery, DoctorLoginResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorLoginQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorLoginResultDto> Handle(DoctorLoginQuery request, CancellationToken cancellationToken)
        {
            var Doctor = await _unitOfWork.DoctorPortalRepository.LoginAsync(
                cancellationToken,
                request.LoginDto.Identifier
            );

            if (Doctor == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            // Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, Doctor.PasswordHash);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            // Clear hash before returning
            Doctor.PasswordHash = null;

            return Doctor;
        }
    }
}
