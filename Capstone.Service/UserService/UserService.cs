using Capstone.Common.DTOs.User;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly CapstoneContext _context;
        private readonly IUserRepository _userRepository;

        public UserService(CapstoneContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUserRequest createUserRequest)
        {
            using var transaction = _userRepository.DatabaseTransaction();
            try
            {
                var newUserRequest = new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = createUserRequest.UserName,
                    Password = createUserRequest.Password,
                    IsAdmin = false,
                    JoinedDate = DateTime.UtcNow,
                    IsFirstTime = true,
                    Gender = createUserRequest.Gender,
                    Email = createUserRequest.Email,
                    Status = Common.Enums.StatusEnum.Active,
                };

                var newUser = await _userRepository.CreateAsync(newUserRequest);
                _userRepository.SaveChanges();

                transaction.Commit();

                return new CreateUserResponse
                {
                    IsSucced = true,
                };
            }
            catch (Exception)
            {
                transaction.RollBack();

                return new CreateUserResponse
                {
                    IsSucced = false,
                };
            }
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllUserAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByIdAsync(Guid Id)
        {
            var user = await _userRepository.GetAsync(user => user.UserId == Id, null);

            if (user == null || Id == null) return null;

            return new User
            {
                UserId = user.UserId,
                Email = user.Email,
                Gender = user.Gender,
                Avatar = user.Avatar,
                JoinedDate = user.JoinedDate,
                Status = user.Status,
                IsAdmin = user.IsAdmin, 
                UserName = user.UserName,
            };
        }

        public async Task<User> LoginUser(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == username && x.Password == password);
        }

        public Task<CreateUserResponse> UpdateUserAsync(UpdateUserRequest updateUserRequest, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
