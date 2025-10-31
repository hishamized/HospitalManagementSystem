using HMS.Application.Commands.User;
using HMS.Application.DTO.User;
using HMS.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.User
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpResultDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<VerifyOtpCommandHandler> _logger;

        public VerifyOtpCommandHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<VerifyOtpCommandHandler> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<VerifyOtpResultDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Verifying OTP: {OtpCode}", request.OtpCode);

                var otpResult = await _userRepository.VerifyOtpAsync(request.OtpCode);

                if (otpResult == null || otpResult.UserId <= 0)
                {
                    _logger.LogWarning("Invalid or expired OTP entered: {OtpCode}", request.OtpCode);
                    return new VerifyOtpResultDto
                    {
                        Success = false,
                        Message = "Invalid or expired OTP."
                    };
                }

                _logger.LogInformation("OTP verified successfully for user {UserId}", otpResult.UserId);

                // 🔹 Generate and store new random password in DB
                var newPassword = await _userRepository.ResetPasswordAfterOtpAsync(otpResult.UserId);

                if (string.IsNullOrEmpty(newPassword))
                {
                    _logger.LogWarning("Password reset failed for user {UserId}", otpResult.UserId);
                    return new VerifyOtpResultDto
                    {
                        Success = false,
                        Message = "Password reset failed after OTP verification."
                    };
                }

                // 🔹 Send the new password via email
                try
                {
                    string subject = "Your New Password - HMS Account";
                    string body = $@"
                        <p>Dear {otpResult.FullName ?? "User"},</p>
                        <p>Your OTP has been successfully verified. Your new login password is:</p>
                        <p><b>{newPassword}</b></p>
                        <p>Please log in and change it immediately after logging in for security reasons.</p>
                        <br/>
                        <p>Best regards,<br/>HMS Security Team</p>";

                    await _emailService.SendEmailAsync(otpResult.Email, subject, body);
                    _logger.LogInformation("New password sent via email to {Email}", otpResult.Email);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send new password email to {Email}", otpResult.Email);
                }

                // 🔹 Return the result back to the client
                return new VerifyOtpResultDto
                {
                    Success = true,
                    Message = "OTP verified successfully. A new password has been sent to your email.",
                    NewPassword = newPassword
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP: {OtpCode}", request.OtpCode);
                return new VerifyOtpResultDto
                {
                    Success = false,
                    Message = "An error occurred while verifying the OTP."
                };
            }
        }
    }
}
