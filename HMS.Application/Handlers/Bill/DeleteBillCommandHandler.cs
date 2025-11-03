using HMS.Application.Commands.Bill;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bill
{
    public class DeleteBillCommandHandler : IRequestHandler<DeleteBillCommand, int>
    {
        private readonly IBillRepository _billRepository;

        public DeleteBillCommandHandler(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }

        public async Task<int> Handle(DeleteBillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Call repository to delete the bill by ID
                var result = await _billRepository.DeleteBillAsync(request.Id);

                if (result <= 0)
                    throw new ApplicationException($"No bill found with Id {request.Id} to delete.");

                return result; // number of affected rows
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting bill: {ex.Message}", ex);
            }
        }
    }
}
