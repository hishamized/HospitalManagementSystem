using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bill
{
    public class DeleteBillCommand : IRequest<int>
    {
        public int Id { get; }

        public DeleteBillCommand(int id)
        {
            Id = id;
        }
    }
}
