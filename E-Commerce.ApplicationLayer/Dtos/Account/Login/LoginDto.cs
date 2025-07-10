using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ApplicationLayer.Dtos.Account.Login
{
    public class LoginDto
    {
        [Required , Display(Name = "Enter Email or User Name")]
        public string EmailOrUsername { get; set; }
        [Required , DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
