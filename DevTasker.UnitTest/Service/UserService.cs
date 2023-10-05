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

namespace DevTasker.UnitTest.Service
{
    [TestFixture]
    public class UserServiceTest
    {
        private IUserService _userService;
        private Mock<CapstoneContext> _contextMock;
        private Mock<IUserRepository> _userRepositoryMock;


        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<CapstoneContext>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_contextMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public async Task TestLoginUserAsync()
        {
            // Arrange
            var username = "admin";
            var password = "1";
            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = username,
                Password = password
            };

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(username, result.UserName);
            Assert.AreEqual(password, result.Password);
        }

        [TearDown]
        public void TearDown()
        {
            // Reset mocks if needed
            _contextMock.Reset();
            _userRepositoryMock.Reset();
        }
    }
}
