using System;

namespace HMS.Application.DTO.Chat
{
    public class MessageDtoDetailed
    {
        // From Message entity
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool IsEdited { get; set; }
        public bool IsDeletedForEveryone { get; set; }

        // From Sender (User entity via JOIN)
        public string SenderUsername { get; set; } = string.Empty;
        public string SenderFullName { get; set; } = string.Empty;

        // From MessageStatus entity
        public int UserId { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsSeen { get; set; }
        public DateTime? SeenAt { get; set; }

        // From User entity (recipient via MessageStatus JOIN)
        public string RecipientUsername { get; set; } = string.Empty;
        public string RecipientFullName { get; set; } = string.Empty;
    }
}