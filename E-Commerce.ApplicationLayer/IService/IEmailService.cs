
namespace E_Commerce.ApplicationLayer.IService
{
    public interface IEmailService
    {
        Task<string> SendEmailAsync(string toEmail, string Message , string bodyHtml);
    }
}
