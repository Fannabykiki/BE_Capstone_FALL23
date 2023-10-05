using Capstone.Common.Enums;

namespace Capstone.Common.DTOs.User
{
    public class UpdateUserRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public GenderEnum Gender { get; set; } //Male = 0,  Female = 1, Other = 2
        public bool IsFirstTime { get; set; } //true
    }
}
