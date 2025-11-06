using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repositories
{
    public class HttpUserContext : IUserContext
    {
        private readonly IHttpContextAccessor _acc;
        public HttpUserContext(IHttpContextAccessor acc) { _acc = acc; }
        public int UserId => int.Parse(_acc.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
    internal class UserContext
    {
    }
}
