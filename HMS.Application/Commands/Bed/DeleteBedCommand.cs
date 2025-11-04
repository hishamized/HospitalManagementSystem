using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bed
{
    public class DeleteBedCommand : IRequest<int>
    {
        public int Id { get; set; }

        public DeleteBedCommand(int id)
        {
            Id = id;
        }
    }
}
