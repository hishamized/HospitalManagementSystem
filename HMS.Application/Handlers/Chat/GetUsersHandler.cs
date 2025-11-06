using HMS.Application.DTO.Chat;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Chat
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, List<UserListDto>>
    {
        private readonly IUserRepository _repo;
        private readonly IPresenceTracker _presence;
        public GetUsersHandler(IUserRepository repo, IPresenceTracker presence) { _repo = repo; _presence = presence; }

        public async Task<List<UserListDto>> Handle(GetUsersQuery req, CancellationToken ct)
        {
            var users = await _repo.SearchActiveUsersAsync(req.Query);
            foreach (var u in users)
            {
                // Online status
                u.IsOnline = _presence.IsOnline(u.Id);

                // Last seen
                // Assuming your repo has a method to get the most recent last seen for a user
                var lastSeen = await _repo.GetLastSeenForUserAsync(u.Id);
                u.LastSeenAt = lastSeen;
            }
            return users;
        }
    }
}
