using HMS.Application.Commands.Chat;
using HMS.Application.Interfaces;
using HMS.Domain.Entities; // Adjust based on where ChatRoomUser is
using HMS.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HMS.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IPresenceTracker _presence;
        private readonly ApplicationDbContext _db; // Inject your DbContext
        private readonly IMediator _mediator;

        public ChatHub(IPresenceTracker presence, ApplicationDbContext db, IMediator mediator)
        {
            _presence = presence;
            _db = db;
            _mediator = mediator;
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
            int userId = GetUserId(); // Extract from Context.User.Claims

            // 1. Broadcast immediately (non-blocking, real-time)
            var messagePayload = new
            {
                chatRoomId,
                senderId = userId,
                content,
                sentAt = DateTime.UtcNow
            };

            await Clients.Group($"chat-{chatRoomId}").SendAsync("ReceiveMessage", messagePayload);

            var cmd = new SendMessageCommand
            {
                ChatRoomId = chatRoomId,
                //SenderId = userId,
                Content = content
            };

            var sentMessage = await _mediator.Send(cmd);
        }


        public Task Typing(int chatRoomId, int userId, bool isTyping)
            => Clients.GroupExcept($"chat-{chatRoomId}", Context.ConnectionId)
                      .SendAsync("UserTyping", userId, isTyping);

        private int GetUserId()
        {
            var userIdClaim = Context.User?.Claims?.FirstOrDefault(c =>
                c.Type == "UserId" || c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) && userId > 0)
            {
                return userId;
            }

            if (!string.IsNullOrEmpty(Context.UserIdentifier) &&
                int.TryParse(Context.UserIdentifier, out var identifierId))
            {
                return identifierId;
            }

            throw new HubException("Unable to resolve the current user's ID. Ensure authentication is properly configured.");
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


        public async Task MessageDelivered(int messageId)
        {
            int userId = GetUserId();

            var cmd = new UpdateMessageStatusCommand
            {
                MessageId = messageId,
                UserId = userId,
                IsDelivered = true
            };

            await _mediator.Send(cmd);

            // Optionally notify the sender in real-time:
            await Clients.User(userId.ToString()).SendAsync("MessageDeliveredAck", messageId);
        }
        public async Task MessagesSeen(List<int> messageIds)
        {
            int userId = GetUserId();

            // 1. Find the senders of these messages (before updating status)
            var senderIds = await _db.Messages
                .Where(m => messageIds.Contains(m.Id))
                .Select(m => m.SenderId)
                .Distinct()
                .ToListAsync();

            // 2. Update the database
            var cmd = new MarkMessagesSeenCommand
            {
                UserId = userId,
                MessageIds = messageIds
            };

            await _mediator.Send(cmd);

            // 3. Notify only the message senders (not ALL users)
            foreach (var senderId in senderIds)
            {
                await Clients.User(senderId.ToString())
                    .SendAsync("MessagesSeenUpdate", new { readByUserId = userId, messageIds });
            }
        }

    }
}
