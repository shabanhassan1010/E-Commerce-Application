using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer.EmailSettings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Runtime;

namespace E_Commerce.ApplicationLayer.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting emailSetting;
        private readonly UserManager<User> _userManager;

        public EmailService(IOptions<EmailSetting> options , UserManager<User> userManager)
        {
            this.emailSetting = options.Value;
            _userManager = userManager;
        }
        public async Task<string> SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    if (string.IsNullOrWhiteSpace(emailSetting.SmtpServer))
                        throw new ArgumentException("SMTP Server is empty");

                    await client.ConnectAsync(emailSetting.SmtpServer, emailSetting.Port, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(emailSetting.Username, emailSetting.Password);

                    var builder = new BodyBuilder
                    {
                        HtmlBody = bodyHtml,
                        TextBody = "Welcome"
                    };

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(emailSetting.SenderName, emailSetting.SenderEmail));
                    message.To.Add(new MailboxAddress("User", toEmail));
                    message.Subject = subject;
                    message.Body = builder.ToMessageBody();

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return $"Failed: {ex.Message}";
            }
        }
        public async Task<string> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "User Not Found";

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return "فشل تأكيد البريد الإلكتروني.";

            return "Success";
        }
    }
}
