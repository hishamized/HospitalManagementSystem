using AutoMapper;
using HMS.Application.Commands.User;
using HMS.Application.Interfaces;
using MediatR;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.User
{
    public class VerifyEmailPhoneCommandHandler : IRequestHandler<VerifyEmailPhoneCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly Interfaces.IEmailService _emailService;
        private readonly IMapper _mapper;

        public VerifyEmailPhoneCommandHandler(
            IUserRepository userRepository,
            Interfaces.IEmailService emailService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<bool> Handle(VerifyEmailPhoneCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Check user and generate OTP in a single atomic DB call
            var result = await _userRepository.VerifyEmailPhoneAsync(request.VerifyEmailPhoneDto);

            if (result == null)
            {
                // No user found with given email + phone
                return false;
            }

            // Step 2: Send OTP email
            try
            {
                string subject = "HMS - Password Reset OTP";
                string body = $"Dear user,\n\nYour OTP for resetting your HMS account password is: {result.OtpCode}\n\n" +
                              "This OTP will expire in 5 minutes.\n\nBest regards,\nHMS Support";

                await _emailService.SendEmailAsync(result.Email, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                // If email fails, we could optionally mark OTP as invalid/used.
                // Depending on your design, you can add a rollback mechanism in the repo.
                // For now, we’ll just log and return false.
                Console.WriteLine($"Error sending OTP email: {ex.Message}");
                return false;
            }
        }
    }
}
