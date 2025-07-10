
namespace E_Commerce.ApplicationLayer.Dtos.Users
{
    public class UserDetailsDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public List<string> Roles { get; set; }
    }
}
