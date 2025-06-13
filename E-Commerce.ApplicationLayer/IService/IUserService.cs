using E_Commerce.ApplicationLayer.Dtos.Account;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.ApplicationLayer.IService
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<TokenDto?> LoginAsync(LoginDto dto);
    }
}
