using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public int SenderId { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? EditedAt { get; set; }
        public bool IsEdited { get; set; } = false;
        public bool IsDeletedForEveryone { get; set; } = false;

        // Navigation properties
        public ChatRoom ChatRoom { get; set; } = null!;
        public User Sender { get; set; } = null!;
        public ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
    }

}
