using AutoMapper;
using HMS.Application.Commands.Bill;
using HMS.Application.DTO.Bill;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bill
{
    public class EditBillCommandHandler : IRequestHandler<EditBillCommand, int>
    {
        private readonly IBillRepository _billRepository;
        private readonly IMapper _mapper;

        public EditBillCommandHandler(IBillRepository billRepository, IMapper mapper)
        {
            _billRepository = billRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(EditBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Directly use the DTO from command
                var result = await _billRepository.UpdateBillAsync(request.Bill);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating bill: {ex.Message}", ex);
            }
        }

    }
}
