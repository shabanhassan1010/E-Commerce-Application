using System.ComponentModel.DataAnnotations;
using E_Commerce.DomainLayer.Entities;

namespace E_Commerce.ApplicationLayer.Dtos.Account
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public AppRole Role { get; set; }
    }
}
