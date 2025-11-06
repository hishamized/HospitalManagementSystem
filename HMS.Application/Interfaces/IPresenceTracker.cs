using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IPresenceTracker
    {
        Task UserConnected(int userId, string connectionId);
        Task UserDisconnected(int userId, string connectionId);
        bool IsOnline(int userId);
        Task<List<string>> GetConnections(int userId);
    }
}
