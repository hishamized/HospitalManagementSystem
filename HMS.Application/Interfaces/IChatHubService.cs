using HMS.Application.DTO.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IChatHubService
    {
        Task SendMessageToGroupAsync(int chatRoomId, MessageDto message);
        Task NotifyUserOnlineAsync(int userId);
        Task NotifyUserOfflineAsync(int userId);
    }
}
