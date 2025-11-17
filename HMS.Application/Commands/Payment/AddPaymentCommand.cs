using HMS.Application.DTO.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Payment
{
    public class AddPaymentCommand : IRequest<int>
    {
        public readonly AddPaymentDto Dto;
        public AddPaymentCommand(AddPaymentDto dto) { 
            Dto = dto;
        }
    }
}
