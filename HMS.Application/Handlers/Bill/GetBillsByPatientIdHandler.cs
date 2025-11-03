using AutoMapper;
using HMS.Application.DTO.Bill;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Bill;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bill
{
    public class GetBillsByPatientIdHandler : IRequestHandler<GetBillsByPatientIdQuery, List<BillListDto>>
    {
        private readonly IBillRepository _billRepository;
        private readonly IMapper _mapper;

        public GetBillsByPatientIdHandler(IBillRepository billRepository, IMapper mapper)
        {
            _billRepository = billRepository;
            _mapper = mapper;
        }

        public async Task<List<BillListDto>> Handle(GetBillsByPatientIdQuery request, CancellationToken cancellationToken)
        {
            // Fetch bill entities from repository
            var bills = await _billRepository.GetBillsByPatientIdAsync(request.PatientId);

            // Map entities to DTOs
            var billDtos = _mapper.Map<List<BillListDto>>(bills);

            return billDtos;
        }
    }
}
