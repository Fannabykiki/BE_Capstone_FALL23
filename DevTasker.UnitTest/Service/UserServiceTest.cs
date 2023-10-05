using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.DataAccess;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Moq;
using Capstone.Service.UserService;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Capstone.DataAccess.Entities;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace DevTasker.UnitTest.Service
{
    
    public class UserServiceTest
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private UserService _userService;


        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(null, _userRepositoryMock.Object);
        }

        [Test]
        public async Task TestLoginUserAsync()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";

            byte[] passwordSalt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(passwordSalt);
            }

            byte[] passwordHash;
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
         .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            if (result != null)
            {
                Console.WriteLine("Success: Login was successful.");
            }
            else
            {
                Console.WriteLine("Wrong: Login was unsuccessful.");
            }
            Assert.NotNull(result);
            Assert.AreEqual(username, result.UserName);
        }
        [Test]
        public async Task TestLoginUserAsync_InvalidCredentials()
        {
            // Arrange
            var username = "testuser";
            var correctPassword = "testpassword";
            var incorrectPassword = "wrongpassword";

            byte[] passwordSalt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(passwordSalt);
            }

            byte[] passwordHash;
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(correctPassword));
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginUser(username, incorrectPassword);

            // Assert
            if (result != null)
            {
                Console.WriteLine("Success: Login was successful.");
            }
            else
            {
                Console.WriteLine("Wrong: Login was unsuccessful.");
            }
            Assert.Null(result);
        }

        [Test]
        public async Task TestLoginUserAsync_UsernameNotFound()
        {
            // Arrange
            var username = "nonexistentuser";
            var password = "testpassword";

            // Giả lập không có người dùng cùng tên tồn tại
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            if (result != null)
            {
                Console.WriteLine("Success: Login was successful.");
            }
            else
            {
                Console.WriteLine("Wrong: Username not found.");
            }
            Assert.Null(result);
        }


    }
}
