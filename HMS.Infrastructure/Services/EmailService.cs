using HMS.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace HMS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;  // change if using another provider
        private readonly int _smtpPort;
        private readonly string _smtpUser;   // move to appsettings.json
        private readonly string _smtpPass;      // move to secrets

        public EmailService(IConfiguration config)
        {
            // try top-level keys first, then fall back to "EmailSettings" section
            var section = config.GetSection("EmailSettings");
            _smtpHost = config["SmtpHost"] ?? section["SmtpHost"];
            var portStr = config["SmtpPort"] ?? section["SmtpPort"];
            _smtpUser = config["SmtpUser"] ?? section["SmtpUser"];
            _smtpPass = config["SmtpPass"] ?? section["SmtpPass"];

            if (string.IsNullOrWhiteSpace(_smtpHost))
                throw new InvalidOperationException("SmtpHost missing from configuration.");
            if (!int.TryParse(portStr, out _smtpPort) || _smtpPort <= 0)
                throw new InvalidOperationException($"SmtpPort is missing or invalid. Value='{portStr}'");
            if (string.IsNullOrWhiteSpace(_smtpUser) || string.IsNullOrWhiteSpace(_smtpPass))
                throw new InvalidOperationException("SmtpUser/SmtpPass missing from configuration.");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HMS Support", _smtpUser));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
