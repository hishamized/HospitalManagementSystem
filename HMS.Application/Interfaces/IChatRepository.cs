using HMS.Application.DTO.Chat;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatRoom> GetOrCreateOneToOneChatAsync(int currentUserId, int partnerId);
        Task<RecentChatDto> GetRecentChatDtoAsync(int chatRoomId, int currentUserId);
        Task<List<RecentChatDto>> GetRecentChatsForUserAsync(int userId);
        Task<List<MessageDtoDetailed>> GetMessagesAsync(int chatRoomId, int page, int pageSize);
        Task<MessageDto> AddMessageAsync(int chatRoomId, int senderId, string content);
        Task<int> ResolveUserIdByIdentifier(string? identifier);

        Task UpdateMessageStatusAsync(int messageId, int userId, bool isDelivered, bool isSeen);
        Task<int> MarkMessagesSeenAsync(int userId, List<int> messageIds);

        Task<IEnumerable<MessageDto>> GetUnseenMessagesAsync(int chatRoomId, int userId, CancellationToken cancellationToken = default);
        Task<int> MarkMessagesAsDeliveredAsync(int chatRoomId, int userId, CancellationToken cancellationToken = default);
        Task DeleteMessageAsync(int messageId, int userId, CancellationToken cancellationToken = default);

        Task EditMessageAsync(int messageId, string newContent, int userId, CancellationToken cancellationToken = default);
    }
}
