using HMS.Application.Commands.DoctorPortal;
using HMS.Application.DTO.DoctorPortal;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorPortal
{
    public class FetchPatientsByDoctorCommandHandler : IRequestHandler<FetchPatientsByDoctorCommand, IEnumerable<FetchPatientsByDoctorDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public FetchPatientsByDoctorCommandHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<FetchPatientsByDoctorDto>> Handle(FetchPatientsByDoctorCommand request, CancellationToken cancellationToken) {
            var result = await _unitOfWork.DoctorPortalRepository.FetchPatientsByDoctorAsync(request, cancellationToken);
            return result;
        }
    }
}
