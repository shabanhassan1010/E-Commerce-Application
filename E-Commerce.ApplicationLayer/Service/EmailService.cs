using E_Commerce.ApplicationLayer.IService;
using E_Commerce.InfrastructureLayer.EmailSettings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Runtime;

namespace E_Commerce.ApplicationLayer.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting emailSetting;
        public EmailService(EmailSetting emailSetting)
        {
            this.emailSetting = emailSetting;
        }
        public async Task<string> SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(emailSetting.SenderName, emailSetting.SenderEmail));
                message.To.Add(new MailboxAddress("User", toEmail));
                message.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = bodyHtml,
                    TextBody = "Welcome"
                };

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(emailSetting.SmtpServer, emailSetting.Port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(emailSetting.Username , emailSetting.Password);
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
    }
}
