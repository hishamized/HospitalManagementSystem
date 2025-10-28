using AutoMapper;
using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Features.Users.Commands.CreateAdmin
{
    public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, (bool Success, string Message)>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateAdminCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message)> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Hash the password before saving
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Admin.Password);


                // Call repository method with DTO + hash
                var rowsAffected = await _userRepository.CreateAdminAsync(request.Admin, passwordHash);

                return rowsAffected > 0
                    ? (true, "Admin created successfully.")
                    : (false, "Failed to create admin. Please try again.");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}
