﻿#region MyRegion
using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.Account.ForgetPassword;
using E_Commerce.ApplicationLayer.Dtos.Account.Login;
using E_Commerce.ApplicationLayer.Dtos.Account.LogOut;
using E_Commerce.ApplicationLayer.Dtos.Account.Rigster;
using E_Commerce.ApplicationLayer.Dtos.Email;
using E_Commerce.ApplicationLayer.Dtos.Users;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Web;
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
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager,
            IConfiguration config,IMapper mapper,
            RoleManager<IdentityRole> roleManager , 
            IUserRepository userRepository , IEmailService emailService , ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            this.mapper = mapper;
            _roleManager = roleManager;
            this.userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
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
                UserName = dto.Email.Split('@')[0],
                PhoneNumber = dto.PhoneNumber
            };

            // Create user account
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)  // if my data match validation
            {
                _logger.LogWarning("User creation failed: {Errors}", string.Join(", ", result.Errors));
                return result;
            }

            // Ensure roles exist
            await EnsureRolesExist();

            // Assign role to user
            var roleResult = await AssignUserRole(user, dto.Role);
            if (!roleResult.Succeeded)
                return roleResult;

            // Add claims
            var claimResult = await AddUserClaims(user);

            // / Generate and send confirmation email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encoded = HttpUtility.UrlEncode(token);
            var confirmationLink = $"https://localhost:7036/api/Email/confirm-email?userId={user.Id}&token={HttpUtility.UrlEncode(encoded)}";
            Console.WriteLine("Confirm Link: " + confirmationLink);
            var subject = "تأكيد البريد الإلكتروني";
            var body = $"<p>مرحبًا {user.FirstName}،</p><p>اضغط على الرابط التالي لتأكيد بريدك الإلكتروني:</p><p><a href='{confirmationLink}'>تأكيد الحساب</a></p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
            return claimResult;
        }
        public async Task<TokenDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.EmailOrUsername)
                ?? await _userManager.FindByNameAsync(dto.EmailOrUsername);
            if (user == null)
                return new TokenDto
                {
                    Token = string.Empty, Message = "User not Found"
                };

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return new TokenDto
                {
                    Token = string.Empty, Message = "Email is not submitted"
                };

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
            {
                return new TokenDto
                {
                    Token = string.Empty,  Message = "Password is not correct"
                };
            }

            var claimsList = await _userManager.GetClaimsAsync(user);

            //  Add user role as a claim
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            if (!string.IsNullOrEmpty(role))
            {
                claimsList.Add(new Claim(ClaimTypes.Role, role));
            }

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

            return new TokenDto { Token = new JwtSecurityTokenHandler().WriteToken(token) , Message = "Success" };
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
        public async Task<PaginationResponse<UserDetailsDto>> GetUsersAsync(int page, int size , string? search = null)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Email.ToLower().Contains(search) ||
                    u.UserName.ToLower().Contains(search) ||
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search)
                );
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var userList = new List<UserDetailsDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new UserDetailsDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    //ProfilePicture = user.pr,
                    Roles = roles.ToList()
                });
            }

            return new PaginationResponse<UserDetailsDto>(page , size , totalCount,userList);
        }
        public async Task<IdentityResult> ChangeUserRoleAsync(string userId, string newRole)
        {
            if(!await _roleManager.RoleExistsAsync(newRole))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Role Is not Exist"
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Role Is not Exist"
                });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var RemoveAllroles = await _userManager.RemoveFromRolesAsync(user ,currentRoles);
            if (!RemoveAllroles.Succeeded)
                return RemoveAllroles;

            // Add New Role
            var NewRole = await _userManager.AddToRoleAsync(user , newRole);
            return NewRole;
        }

        #region Comman Methods
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

        //private async Task SendEmailConfirmation(User user)
        //{
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var encodedToken = HttpUtility.UrlEncode(token);

        //    var confirmationLink = $"{_config["ClientApp:BaseUrl"]}/confirm-email?userId={user.Id}&token={encodedToken}";

        //    var subject = "Confirm Your Email";
        //    var body = $@"<p>Hello {user.FirstName},</p>
        //                <p>Please confirm your email by clicking the link below:</p>
        //                <p><a href='{confirmationLink}'>Confirm Email</a></p>
        //                <p>This link will expire in 24 hours.</p>";

        //    await _emailService.SendEmailAsync(user.Email, subject, body);
        //    _logger.LogInformation("Confirmation email sent to {Email}", user.Email);
        //}

        #endregion
    }
}
