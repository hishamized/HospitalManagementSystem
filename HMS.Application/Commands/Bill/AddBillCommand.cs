using HMS.Application.DTO.Bill;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bill
{
    public class AddBillCommand : IRequest<int>
    {
        public AddBillDto BillDto { get; set; }

        public AddBillCommand(AddBillDto billDto)
        {
            BillDto = billDto;
        }
    }
}
