using AutoMapper;
using Dapper;
using HMS.Application.DTO.Chat;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using System.Data;


namespace HMS.Infrastructure.Repositories
{
    internal class ChatRepository : IChatRepository
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;

        public ChatRepository(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ChatRoom> GetOrCreateOneToOneChatAsync(int a, int b)
        {
            using var conn = _context.CreateConnection();

            try
            {
                var chat = await conn.QueryFirstOrDefaultAsync<ChatRoom>(
                    "sp_GetOrCreateOneToOneChat",
                    new { UserA = a, UserB = b },
                    commandType: CommandType.StoredProcedure);

                return chat;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in GetOrCreateOneToOneChatAsync: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                throw;
            }
        }



        public async Task<MessageDto> AddMessageAsync(int chatRoomId, int senderId, string content)
        {
            using var conn = _context.CreateConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();

            try
            {
                var msg = await conn.QuerySingleAsync<MessageDto>(
                    "sp_AddNewMessage",
                    new { ChatRoomId = chatRoomId, SenderId = senderId, Content = content },
                    commandType: CommandType.StoredProcedure
                );

                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in AddMessageAsync: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
                throw;
            }
        }

        public async Task UpdateMessageStatusAsync(int messageId, int userId, bool isDelivered, bool isSeen)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@MessageId", messageId, DbType.Int32);
            parameters.Add("@UserId", userId, DbType.Int32);
            parameters.Add("@IsDelivered", isDelivered, DbType.Boolean);
            parameters.Add("@IsSeen", isSeen, DbType.Boolean);

            await connection.ExecuteAsync(
                "sp_UpdateMessageStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> MarkMessagesSeenAsync(int userId, List<int> messageIds)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@MessageIds", string.Join(",", messageIds));

            var result = await connection.ExecuteScalarAsync<int>(
                "sp_MarkMessagesSeen",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<IEnumerable<MessageDto>> GetUnseenMessagesAsync(int chatRoomId, int userId, CancellationToken cancellationToken = default)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ChatRoomId", chatRoomId, DbType.Int32);
            parameters.Add("@UserId", userId, DbType.Int32);

            var messages = await conn.QueryAsync<MessageDto>(
                "sp_GetUnseenMessages",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return messages;
        }

        public async Task EditMessageAsync(int messageId, string newContent, int userId, CancellationToken cancellationToken = default)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@MessageId", messageId, DbType.Int32);
            parameters.Add("@NewContent", newContent, DbType.String);
            parameters.Add("@UserId", userId, DbType.Int32);

            await conn.ExecuteAsync(
                "sp_EditMessage",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<RecentChatDto> GetRecentChatDtoAsync(int chatRoomId, int currentUserId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
SELECT
    cr.Id AS ChatRoomId,
    CASE WHEN cr.IsGroup = 1 THEN ISNULL(cr.Name, '') 
         ELSE ISNULL(u.FullName, '') END AS Title,
    ISNULL(m.Content, '') AS LastMessageSnippet,
    ISNULL(m.SentAt, cr.CreatedAt) AS LastUpdatedAt,
    ISNULL(ms.UnreadCount, 0) AS UnreadCount
FROM ChatRooms cr
OUTER APPLY (
    SELECT TOP 1 Content, SentAt
    FROM Messages
    WHERE ChatRoomId = cr.Id AND IsDeletedForEveryone = 0
    ORDER BY SentAt DESC, Id DESC
) m
OUTER APPLY (
    SELECT TOP 1 u.FullName
    FROM ChatRoomUsers cru
    JOIN Users u ON u.Id = cru.UserId
    WHERE cru.ChatRoomId = cr.Id AND u.Id <> @CurrentUserId
) u
LEFT JOIN (
    SELECT m.ChatRoomId, SUM(CASE WHEN ms.IsSeen = 0 THEN 1 ELSE 0 END) AS UnreadCount
    FROM Messages m
    JOIN MessageStatuses ms ON ms.MessageId = m.Id
    WHERE ms.UserId = @CurrentUserId
    GROUP BY m.ChatRoomId
) ms ON ms.ChatRoomId = cr.Id
WHERE cr.Id = @ChatRoomId;
";
            var dto = await conn.QueryFirstOrDefaultAsync<RecentChatDto>(sql, new { ChatRoomId = chatRoomId, CurrentUserId = currentUserId });
            return dto ?? new RecentChatDto { ChatRoomId = chatRoomId, Title = string.Empty, LastMessageSnippet = string.Empty, UnreadCount = 0, LastUpdatedAt = DateTime.UtcNow };
        }

        public async Task<List<RecentChatDto>> GetRecentChatsForUserAsync(int userId)
        {
            using var conn = _context.CreateConnection();

            var sql = @"
SELECT
    cr.Id AS ChatRoomId,
    CASE WHEN cr.IsGroup = 1 THEN ISNULL(cr.Name, '')
         ELSE ISNULL(otherUser.FullName, '') END AS Title,
    ISNULL(latest.Content, '') AS LastMessageSnippet,
    ISNULL(latest.SentAt, cr.CreatedAt) AS LastUpdatedAt,
    ISNULL(unread.UnreadCount, 0) AS UnreadCount
FROM ChatRooms cr
JOIN ChatRoomUsers cru ON cru.ChatRoomId = cr.Id
LEFT JOIN (
    SELECT m.ChatRoomId, m.Content, m.SentAt
    FROM Messages m
    WHERE m.Id IN (
        SELECT MAX(Id) FROM Messages WHERE ChatRoomId IN (SELECT ChatRoomId FROM ChatRoomUsers WHERE UserId = @UserId) GROUP BY ChatRoomId
    )
) latest ON latest.ChatRoomId = cr.Id
LEFT JOIN (
    SELECT m.ChatRoomId, COUNT(*) AS UnreadCount
    FROM Messages m
    JOIN MessageStatuses ms ON ms.MessageId = m.Id
    WHERE ms.UserId = @UserId AND ms.IsSeen = 0
    GROUP BY m.ChatRoomId
) unread ON unread.ChatRoomId = cr.Id
OUTER APPLY (
    SELECT u.Id, u.FullName
    FROM ChatRoomUsers cru2
    JOIN Users u ON u.Id = cru2.UserId
    WHERE cru2.ChatRoomId = cr.Id AND u.Id <> @UserId
) otherUser
WHERE cru.UserId = @UserId
ORDER BY ISNULL(latest.SentAt, cr.CreatedAt) DESC;
";
            var list = (await conn.QueryAsync<RecentChatDto>(sql, new { UserId = userId })).ToList();
            return list;
        }

        public async Task<List<MessageDtoDetailed>> GetMessagesAsync(int chatRoomId, int page, int pageSize)
        {
            using var conn = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ChatRoomId", chatRoomId);
            parameters.Add("@Page", page);
            parameters.Add("@PageSize", pageSize);
            var result = await conn.QueryAsync<MessageDtoDetailed>("sp_GetMessages", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<int> ResolveUserIdByIdentifier(string? identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier cannot be null or empty.", nameof(identifier));

            using var conn = _context.CreateConnection();
            // Try to resolve by Username first, then by Email
            var sql = @"
                SELECT Id FROM Users WHERE Username = @identifier OR Email = @identifier
            ";
            var userId = await conn.QueryFirstOrDefaultAsync<int?>(sql, new { identifier });
            if (userId.HasValue)
                return userId.Value;

            throw new InvalidOperationException($"User not found for identifier '{identifier}'.");
        }

        public async Task<int> MarkMessagesAsDeliveredAsync(int chatRoomId, int userId, CancellationToken cancellationToken = default)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int32);

            var count = await conn.QuerySingleAsync(
                "sp_MarkMessagesAsDelivered",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count;
        }
        public async Task DeleteMessageAsync(int messageId, int userId, CancellationToken cancellationToken = default)
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@MessageId", messageId, DbType.Int32);
            parameters.Add("@UserId", userId, DbType.Int32);

            await conn.ExecuteAsync(
                "sp_DeleteMessage",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
