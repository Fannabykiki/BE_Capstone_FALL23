using Capstone.Common.DTOs.User;
using Capstone.Common.Token;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.UserService
{
    public interface IUserService
    {
        Task<User> LoginUser(string username, string password);
        Task<User> GetUserByIdAsync(Guid Id);
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<bool> DeleteAsync(Guid id);
        Task<CreateUserResponse> UpdateUserAsync(UpdateUserRequest updateUserRequest, Guid id);
        Task<CreateUserResponse> CreateAsync(CreateUserRequest createUserRequest);
        Task<RefreshToken> GenerateRefreshToken();
        Task SetRefreshToken(RefreshToken newRefreshToken);
    }
}
