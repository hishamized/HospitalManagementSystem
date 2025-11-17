using AutoMapper;
using HMS.Application.DTO.PatientPortal;
using HMS.Application.Queries.Bill;
using HMS.Domain.Interfaces;
using MediatR;


namespace HMS.Application.Handlers.Bill
{
    public class GetPatientBillsQueryHandler : IRequestHandler<GetPatientBillsQuery, IEnumerable<GetPatientBillsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPatientBillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetPatientBillsDto>> Handle(GetPatientBillsQuery request, CancellationToken cancellationtoken) {
            var result = await _unitOfWork.PatientPortalRepository.GetPatientBillsAsync(request.PatientId, request.VisitId, cancellationtoken);
            return result;
        }
    }
}
