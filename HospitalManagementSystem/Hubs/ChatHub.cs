using HMS.Application.Interfaces;
using HMS.Domain.Entities; // Adjust based on where ChatRoomUser is
using HMS.Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HMS.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IPresenceTracker _presence;
        private readonly ApplicationDbContext _db; // Inject your DbContext

        public ChatHub(IPresenceTracker presence, ApplicationDbContext db)
        {
            _presence = presence;
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            int userId = GetUserId();

            var isAlreadyOnline = _presence.IsOnline(userId); // check BEFORE adding
            await _presence.UserConnected(userId, Context.ConnectionId);

            if (!isAlreadyOnline)
                await Clients.All.SendAsync("UserOnline", userId);

            // Optional: update LastSeenAt for all chat rooms the user is part of
            await UpdateLastSeenForAllChatRooms(userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int userId = GetUserId();

            await _presence.UserDisconnected(userId, Context.ConnectionId);

            if (!_presence.IsOnline(userId))
                await Clients.All.SendAsync("UserOffline", userId);

            // Update LastSeenAt when user disconnects
            await UpdateLastSeenForAllChatRooms(userId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(int chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatRoomId}");

            int userId = GetUserId();

            // Update last seen for this chat room
            await UpdateLastSeen(chatRoomId, userId);
        }

        public async Task LeaveChat(int chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatRoomId}");
        }

        public async Task SendMessage(int chatRoomId, string content)
        {
            int userId = GetUserId();

            // Update last seen whenever the user sends a message
            await UpdateLastSeen(chatRoomId, userId);

            // Broadcast the message to the group
            await Clients.Group($"chat-{chatRoomId}").SendAsync("ReceiveMessage", new
            {
                chatRoomId,
                senderId = userId,
                content,
                sentAt = DateTime.UtcNow
            });
        }

        public Task Typing(int chatRoomId, int userId, bool isTyping)
            => Clients.GroupExcept($"chat-{chatRoomId}", Context.ConnectionId)
                      .SendAsync("UserTyping", userId, isTyping);

        private int GetUserId()
        {
            if (string.IsNullOrEmpty(Context.UserIdentifier))
                throw new HubException("UserIdentifier is not set for this connection.");

            return int.Parse(Context.UserIdentifier);
        }

        // ===== Helper methods to update LastSeen =====
        private async Task UpdateLastSeen(int chatRoomId, int userId)
        {
            var cru = await _db.ChatRoomUsers
                               .FirstOrDefaultAsync(c => c.ChatRoomId == chatRoomId && c.UserId == userId);
            if (cru != null)
            {
                cru.LastSeenAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        private async Task UpdateLastSeenForAllChatRooms(int userId)
        {
            var chatRooms = await _db.ChatRoomUsers
                                     .Where(c => c.UserId == userId)
                                     .ToListAsync();
            foreach (var cru in chatRooms)
                cru.LastSeenAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
    }
}
