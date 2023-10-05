namespace Capstone.Common.DTOs.User
{
    public class LoginResponse
    {
        public string IsAdmin { get; set; }
        public string UserName { get; set; }
        public Guid UserId { get; set; }
    }
}
