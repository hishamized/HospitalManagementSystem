using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Ward
{
    public class DeleteWardCommand : IRequest<int>
    {
        public int Id { get; set; }

        public DeleteWardCommand(int id)
        {
            Id = id;
        }
    }
}
