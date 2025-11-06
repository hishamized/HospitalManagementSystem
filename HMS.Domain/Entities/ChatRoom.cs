using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public bool IsGroup { get; set; } = false;
        public string? Name { get; set; } // For group chats

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = new List<ChatRoomUser>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }

}
