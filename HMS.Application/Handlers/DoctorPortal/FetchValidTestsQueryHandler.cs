using HMS.Application.DTO.DoctorPortal;
using HMS.Application.Queries.DoctorPortal;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.DoctorPortal
{
    public class FetchValidTestsQueryHandler : IRequestHandler<FetchValidTestsQuery, IEnumerable<FetchValidTestsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public FetchValidTestsQueryHandler(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<FetchValidTestsDto>> Handle(FetchValidTestsQuery request, CancellationToken cancellationToken) {
            var result = await _unitOfWork.DoctorPortalRepository.FetchValidTestsAsync(request, cancellationToken);
            return result;
        }
    }
}
