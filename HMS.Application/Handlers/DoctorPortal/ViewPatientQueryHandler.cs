using HMS.Application.DTO.DoctorPortal;
using HMS.Application.Queries.DoctorPortal;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorPortal
{
    public class ViewPatientQueryHandler : IRequestHandler<ViewPatientQuery, ViewPatientDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ViewPatientQueryHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<ViewPatientDto> Handle(ViewPatientQuery request, CancellationToken cancellationToken) {
            var result = await _unitOfWork.DoctorPortalRepository.ViewPatientAsync(request, cancellationToken);
            if (result != null)
            {
                return result;
            }
            else {
                throw new Exception();
            }
        }
    }
}
