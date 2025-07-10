using System.ComponentModel.DataAnnotations;
using E_Commerce.DomainLayer.Entities;

namespace E_Commerce.ApplicationLayer.Dtos.Account.Rigster
{
    public class RegisterDto
    {
        [Required , MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        [Required , EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required , Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
