using E_Commerce.ApplicationLayer.Dtos.Account;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.ApplicationLayer.Service
{
    public class UserService : IUserService
    {
        #region Constructor
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly SignInManager<User> _signInManager;
        public UserService(UserManager<User> userManager, IConfiguration config, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _config = config;
            _signInManager = signInManager;
        }
        #endregion

        // for Admin
        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);


            var claimList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id),
                new Claim(ClaimTypes.Role , "Admin"),
            };
            await _userManager.AddClaimsAsync(user, claimList);

            return IdentityResult.Success;
        }

        public async Task<TokenDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return null;

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            var claimsList = await _userManager.GetClaimsAsync(user);

            // Read JWT settings from appsettings.json
            var secretKey = _config["JwtSettings:Key"];
            var algorthim = SecurityAlgorithms.HmacSha256Signature;

            var KeyInBits = Encoding.ASCII.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(KeyInBits);

            var creds = new SigningCredentials(key, algorthim);

            var token = new JwtSecurityToken(
                claims: claimsList,
                signingCredentials: creds,
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(10)
            );

            return new TokenDto { Token = new JwtSecurityTokenHandler().WriteToken(token) };
        }
    }
}
