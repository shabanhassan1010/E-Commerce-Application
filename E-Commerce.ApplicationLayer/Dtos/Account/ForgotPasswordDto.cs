
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ApplicationLayer.Dtos.Account
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
