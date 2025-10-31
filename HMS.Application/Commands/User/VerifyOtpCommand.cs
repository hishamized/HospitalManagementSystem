using HMS.Application.Dto;
using HMS.Application.DTO.User;
using MediatR;

namespace HMS.Application.Commands.User
{
    public class VerifyOtpCommand : IRequest<VerifyOtpResultDto>
    {
        public string OtpCode { get; }

        public VerifyOtpCommand(string otpCode)
        {
            OtpCode = otpCode;
        }
    }
}
