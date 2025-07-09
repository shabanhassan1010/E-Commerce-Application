using E_Commerce.ApplicationLayer.Dtos.Email;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.ApplicationLayer.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace E_Commerce_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        #region Context
        private readonly IEmailService _emailService;
        private readonly IConfiguration config;

        public EmailController(IEmailService emailService , IConfiguration config)
        {
            _emailService = emailService;
            this.config = config;
        }
        #endregion

        #region SendEmail
        [HttpPost("SendEmail")]
        [EndpointSummary("Send Email")]
        public async Task<IActionResult> SendEmail(EmailRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.To))
                return BadRequest("Please enter your email first");

            var email = await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);

            if (email == null)
                return BadRequest("something Happened please check the error");
            else
                return Ok("Message sent to Your Emali Successfully");
        }
        #endregion

        #region ConfirmEmail
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            Console.WriteLine("UserId: " + userId);
            Console.WriteLine("Token: " + token);
            var result = await _emailService.ConfirmEmailAsync(userId, token);

            if (result != "Success")
                return BadRequest(result);

            // Redirect to frontend login page from configuration
            var loginUrl = config["EmailSettings:LoginUrl"] ?? "https://yourfrontend.com/login";
            return Redirect(loginUrl);
        }
        #endregion
    }
}
