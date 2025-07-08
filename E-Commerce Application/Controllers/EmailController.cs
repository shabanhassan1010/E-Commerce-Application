using E_Commerce.ApplicationLayer.Dtos.Email;
using E_Commerce.ApplicationLayer.IService;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.To))
                return BadRequest("يرجى إدخال البريد الإلكتروني للمستلم.");

            await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);
            return Ok(" تم إرسال البريد بنجاح.");
        }
    }
}
