using AutoMapper;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    internal class PresenceTracker : IPresenceTracker
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DapperContext _context;
        private readonly IMapper _mapper;
        private static readonly ConcurrentDictionary<int, HashSet<string>> _online = new();

        public PresenceTracker(IUnitOfWork unitOfWork, DapperContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
        }
        public Task UserConnected(int userId, string connectionId)
        {
            var set = _online.GetOrAdd(userId, id => new HashSet<string>());
            lock (set) { set.Add(connectionId); }
            return Task.CompletedTask;
        }
        public Task UserDisconnected(int userId, string connectionId)
        {
            if (_online.TryGetValue(userId, out var set))
            {
                lock (set) { set.Remove(connectionId); if (set.Count == 0) _online.TryRemove(userId, out _); }
            }
            return Task.CompletedTask;
        }
        public bool IsOnline(int userId) => _online.ContainsKey(userId);
        public Task<List<string>> GetConnections(int userId) => Task.FromResult(_online.TryGetValue(userId, out var set) ? set.ToList() : new List<string>());
    }
}
