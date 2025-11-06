using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Chat
{
    public class RecentChatDto
    {
        public int ChatRoomId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string LastMessageSnippet { get; set; } = string.Empty;
        public int UnreadCount { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
