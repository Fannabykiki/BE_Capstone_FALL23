using Capstone.Common.DTOs.User;
using Capstone.Common.Token;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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
                var user = await _userRepository.GetAsync(user => user.UserName == createUserRequest.UserName, null);
                if (user == null)
                {
                    CreatePasswordHash(createUserRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    var newUserRequest = new User
                    {
                        UserId = Guid.NewGuid(),
                        UserName = createUserRequest.UserName,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
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
                return null;
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
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public Task<CreateUserResponse> UpdateUserAsync(UpdateUserRequest updateUserRequest, Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> LoginUser(string username, string password)
        {
            var user = await _userRepository.GetAsync(x => x.UserName == username, null);
            if (user != null)
            {
                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    return null;
                }
                else 
                { 
                    return user; 
                }
            }
            return null;
        }

		public async Task<RefreshToken> GenerateRefreshToken()
		{
			var refreshToken = new RefreshToken()
			{
				Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
				Expires = DateTime.UtcNow.AddDays(5),
				Created = DateTime.UtcNow
			};
			return refreshToken;
		}

		public async Task SetRefreshToken(RefreshToken newRefreshToken)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = newRefreshToken.Expires,
			};
		}
	}
}
