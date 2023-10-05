namespace Capstone.Common.DTOs.User
{
    public class LoginResponse
    {
        public bool IsAdmin { get; set; }
        public bool IsFirstTime { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
