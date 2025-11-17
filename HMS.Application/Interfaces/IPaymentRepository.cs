using HMS.Application.Commands.Payment;
using HMS.Application.DTO.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<int> ProcessPaymentAsync(CancellationToken cancellationToken, AddPaymentCommand request);
    }
}
