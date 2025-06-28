using E_Commerce.ApplicationLayer.Dtos.Account.ForgetPassword;
using E_Commerce.ApplicationLayer.Dtos.Account.Login;
using E_Commerce.ApplicationLayer.Dtos.Account.Rigster;
using E_Commerce.ApplicationLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace E_Commerce_Application.Controllers
{
    public class AccountController : BaseApiController
    {
        /// <summary>
        /// UserManager<User> 
        /// [1]  this service will help me to hash password and compare hashpassword with other and save it in database   
        /// </summary>
        /// 
        #region configuration
        private readonly IConfiguration _configuration;
        private readonly IUserService userService;
        public AccountController(IConfiguration configuration , IUserService userService)
        {
            _configuration = configuration;
            this.userService = userService;
        }
        #endregion

        #region Register
        [HttpPost("Register")]
        [EndpointSummary("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userService.RegisterAsync(registerDto);

            if (result.Succeeded)
                return Ok(new { message = "Admin registered successfully." });

            return BadRequest(result.Errors);
        }
        #endregion

        #region Login
        [HttpPost("Login")]
        [EndpointSummary("Login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await userService.LoginAsync(loginDto);

            if (token == null)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(token);
        }
        #endregion

        #region ForgotPassword
        [HttpPost("forgot-password")]
        [EndpointSummary("Forgot Password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userService.ForgotPasswordAsync(dto);
            if (!result)
                return NotFound(new { message = "Email not found." });

            return Ok(new { message = "Password reset token sent to your email (check console in development)." });
        }
        #endregion

        #region ResetPassword
        [HttpPost("reset-password")]
        [EndpointSummary("Reset Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userService.ResetPasswordAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Password has been reset successfully." });
        }
        #endregion

        #region LogOut
        [HttpPost("logout")]
        [EndpointSummary("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            await userService.LogOutAsync(userId);

            return Ok(new { message = "Logged out successfully." });
        }
        #endregion
    }
}
