using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class MessageStatus
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }

        public bool IsDelivered { get; set; } = false;
        public bool IsSeen { get; set; } = false;
        public DateTime? SeenAt { get; set; }

        // Navigation properties
        public Message Message { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
