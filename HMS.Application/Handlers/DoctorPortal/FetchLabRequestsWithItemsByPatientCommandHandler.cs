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
    public class FetchLabRequestsWithItemsByPatientCommandHandler 
        : IRequestHandler<FetchLabRequestsWithItemsByPatientCommand, 
          IEnumerable<FetchLabRequestsWithItemsByPatientDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public FetchLabRequestsWithItemsByPatientCommandHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<FetchLabRequestsWithItemsByPatientDto>> 
        Handle(FetchLabRequestsWithItemsByPatientCommand request
        , CancellationToken cancellationToken) {
            var result = await _unitOfWork.DoctorPortalRepository.FetchLabRequestsWithItemsByPatientAsync(request, cancellationToken);
            return result;
        }
    }
}
