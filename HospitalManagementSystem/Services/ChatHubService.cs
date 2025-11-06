using HMS.Application.DTO.Chat;
using HMS.Application.Interfaces;
using HMS.Web.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace HMS.Web.Services
{
    public class ChatHubService : IChatHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatHubService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToGroupAsync(int chatRoomId, MessageDto message)
        {
            await _hubContext.Clients.Group(chatRoomId.ToString())
                .SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyUserOnlineAsync(int userId)
        {
            await _hubContext.Clients.All.SendAsync("UserOnline", userId);
        }

        public async Task NotifyUserOfflineAsync(int userId)
        {
            await _hubContext.Clients.All.SendAsync("UserOffline", userId);
        }
    }
}
