using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Feedback
{
    public class DeleteFeedbackCommand : IRequest<int>
    {
        public int Id { get; set; }

        public DeleteFeedbackCommand(int id)
        {
            Id = id;
        }
    }
}
