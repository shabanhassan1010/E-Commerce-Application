﻿using E_Commerce.ApplicationLayer.Dtos.Account.ForgetPassword;
using E_Commerce.ApplicationLayer.Dtos.Account.Login;
using E_Commerce.ApplicationLayer.Dtos.Account.LogOut;
using E_Commerce.ApplicationLayer.Dtos.Account.Rigster;
using E_Commerce.ApplicationLayer.Dtos.Users;
using E_Commerce.InfrastructureLayer;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.ApplicationLayer.IService
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<TokenDto?> LoginAsync(LoginDto dto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<UserDto?> LogOutAsync(int userId);
        Task<PaginationResponse<UserDetailsDto>> GetUsersAsync(int page, int size , string? search = null);
        public Task<IdentityResult> ChangeUserRoleAsync(string userId, string newRole);
    }
}
