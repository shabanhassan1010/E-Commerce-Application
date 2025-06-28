#region MyRegion
using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.Account.ForgetPassword;
using E_Commerce.ApplicationLayer.Dtos.Account.Login;
using E_Commerce.ApplicationLayer.Dtos.Account.LogOut;
using E_Commerce.ApplicationLayer.Dtos.Account.Rigster;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
#endregion

namespace E_Commerce.ApplicationLayer.Service
{
    public class UserService : IUserService
    {

        #region  Constructor
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IMapper mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository userRepository;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager,
            IConfiguration config,IMapper mapper,
            RoleManager<IdentityRole> roleManager , 
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            this.mapper = mapper;
            _roleManager = roleManager;
            this.userRepository = userRepository;
        }
        #endregion

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            // Validate email uniqueness
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email is already registered."
                });
            }

            // Create new user
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            // Create user account
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)  // if my data match validation
                return result;

            // Ensure roles exist
            await EnsureRolesExist();

            // Assign role to user
            var roleResult = await AssignUserRole(user, dto.Role);
            if (!roleResult.Succeeded)
                return roleResult;

            // Add claims
            var claimResult = await AddUserClaims(user);
            return claimResult;
        }

        private async Task EnsureRolesExist()
        {
            if (!await _roleManager.RoleExistsAsync(AppRole.Admin.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(AppRole.Admin.ToString()));

            if (!await _roleManager.RoleExistsAsync(AppRole.customer.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(AppRole.customer.ToString()));
        } 

        private async Task<IdentityResult> AssignUserRole(User user, string role)
        {
            // Validate requested role
            if (role != "Admin" && role != "customer")
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidRole",
                    Description = "Invalid role specified"
                });
            }

            // Assign role
            return await _userManager.AddToRoleAsync(user, role);
        }

        private async Task<IdentityResult> AddUserClaims(User user)
        {
            var claims = new List<Claim>   // like identify for any one 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            return await _userManager.AddClaimsAsync(user, claims);
        }

        public async Task<TokenDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return null;

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
                return null;

            var claimsList = await _userManager.GetClaimsAsync(user);

            var secretKey = _config["JwtSettings:Key"];
            var algorithm = SecurityAlgorithms.HmacSha256;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                claims: claimsList,
                signingCredentials: creds,
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(10)
            );

            return new TokenDto { Token = new JwtSecurityTokenHandler().WriteToken(token) };
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // For testing, return token directly. In production, send via email.
            Console.WriteLine($"Reset token: {token}");

            // You can use an email service here.
            return true;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "No user associated with this email."
                });

            return await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        }

        public async Task<UserDto?> LogOutAsync(int userId)
        {
            var user = await userRepository.GetUser(userId);
            if (user == null)
                return null;

            return mapper.Map<UserDto>(user);
        }

    }
}
