using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Chat
{
    public class UpdateMessageStatusCommand : IRequest<Unit>
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsSeen { get; set; }
    }

}
