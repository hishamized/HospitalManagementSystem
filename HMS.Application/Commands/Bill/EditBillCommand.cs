using HMS.Application.DTO.Bill;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bill
{
    public class EditBillCommand : IRequest<int>
    {
        public EditBillDto Bill { get; }

        
        public EditBillCommand(EditBillDto bill)
        {
            Bill = bill;
        }
    }
}
