using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class ChatRoomUser
    {
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }

        public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
        public bool IsTyping { get; set; } = false;

        // Navigation properties
        public ChatRoom ChatRoom { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
