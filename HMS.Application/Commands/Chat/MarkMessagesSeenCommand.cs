using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Chat
{
    using MediatR;
    using System.Collections.Generic;

    public class MarkMessagesSeenCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public List<int> MessageIds { get; set; } = new();
    }

}
