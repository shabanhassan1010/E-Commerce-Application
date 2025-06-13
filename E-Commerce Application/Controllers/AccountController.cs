

using E_Commerce.ApplicationLayer.Dtos.Account;
using E_Commerce.DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion


        #region Admin
        [HttpPost("Register")]
        [EndpointSummary("Register For Admin")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var user = new User
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
            };

            var res = await _userManager.CreateAsync(user, registerDto.Password);
            if (!res.Succeeded)
                return BadRequest(res.Errors);

            await _userManager.AddToRoleAsync(user, "Admin");

            // store Claims
            var claimList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id),
                new Claim(ClaimTypes.Role , "Admin"),
            };
            await _userManager.AddClaimsAsync(user, claimList);
            return NoContent();
        }


        [HttpPost("login")]
        [EndpointSummary("login For Admin")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginDto)
        {

            User? user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            bool isCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isCorrect)
                return Unauthorized("Invalid credentials");


            // JWT claims
            var claimsList = await _userManager.GetClaimsAsync(user);

            // Read JWT settings from appsettings.json
            var secretKey = _configuration["JwtSettings:Key"];
            var algorthim = SecurityAlgorithms.HmacSha256Signature;

            var KeyInBits = Encoding.ASCII.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(KeyInBits);

            var creds = new SigningCredentials(key, algorthim);

            var token = new JwtSecurityToken(
                claims: claimsList,
                signingCredentials: creds,
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(10)
            );

            // now i will convert this token from obj into string
            var tokenDto = new TokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return Ok(tokenDto);
        }
        #endregion
    }
}
