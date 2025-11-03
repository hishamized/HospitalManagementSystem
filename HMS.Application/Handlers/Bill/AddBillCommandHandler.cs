using AutoMapper;
using HMS.Application.Commands.Bill;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bill
{
    public class AddBillCommandHandler : IRequestHandler<AddBillCommand, int>
    {
        private readonly IBillRepository _billRepository;
        private readonly IMapper _mapper;

        public AddBillCommandHandler(IBillRepository billRepository, IMapper mapper)
        {
            _billRepository = billRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.BillDto == null)
                    throw new ArgumentNullException(nameof(request.BillDto), "Bill data cannot be null.");

                // Map DTO to entity (optional, mostly for structure clarity)
                var billEntity = _mapper.Map<Domain.Entities.Bill>(request.BillDto);

                // Insert bill and return its new ID
                var newBillId = await _billRepository.AddBillAsync(request.BillDto);

                if (newBillId <= 0)
                    throw new Exception("Failed to insert bill record.");

                return newBillId;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while adding bill.", ex);
            }
        }
    }
}
