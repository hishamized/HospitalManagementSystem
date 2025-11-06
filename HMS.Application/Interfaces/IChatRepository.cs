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
        Task<List<MessageDto>> GetMessagesAsync(int chatRoomId, int page, int pageSize);
        Task<MessageDto> AddMessageAsync(int chatRoomId, int senderId, string content);
        Task<int> ResolveUserIdByIdentifier(string? identifier);
    }
}
