using AutoMapper;
using HMS.Application.DTO.PatientPortal;
using HMS.Application.Queries.PatientPortal;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.PatientPortal
{
    public class GetPatientVisitsPortalQueryHandler : IRequestHandler<GetPatientVisitsPortalQuery,IEnumerable< GetPatientVisitsResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPatientVisitsPortalQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetPatientVisitsResponseDto>> Handle(GetPatientVisitsPortalQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.PatientPortalRepository.GetPatientVisitsByIdAsync(cancellationToken, request.PatientId);

            return result;
        }

        //public async Task<GetPatientVisitsResponseDto> Handle(GetPatientVisitsPortalQuery request, CancellationToken token) {
        //    var result = await _unitOfWork.PatientPortalRepository.GetPatientVisitsByIdAsync(token, request.PatientId);

        //    return result;
        //}
    }
}
