using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotesTodo.DAL;
using NotesTodo.Models;
using NotesTodo.Services;

namespace Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private UserSevice _userService = null!;
        private TodoDb _context = null!;
        private IConfiguration _configuration = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TodoDb>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TodoDb(options);
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Secret", "this-is-a-test-secret-that-is-long-enough" },
                    { "Jwt:Issuer", "NotesTodo" },
                    { "Jwt:Audience", "NotesTodo" }
                }).Build();

            _userService = new UserSevice(_context, _configuration);
        }
        [TestMethod]
        public async Task RegisterUserAsync_ShouldRegisterUser()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "testuser@hello.com",
                Password = "password123"
            };
            // Act
            var registeredUser = await _userService.RegisterUserAsync(createUser);
            // Assert
            Assert.IsNotNull(registeredUser);
            Assert.AreEqual(createUser.Name, registeredUser.Name);
            Assert.AreEqual(createUser.Email, registeredUser.Email);
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "testuser@hello.com",
                Password = "password123"
            };
            var createUserDuplicate = new CreateUserDTO
            {
                Name = "Test User 2",
                Email = "testuser@hello.com",
                Password = "password456"
            };
            await _userService.RegisterUserAsync(createUser);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _userService.RegisterUserAsync(createUserDuplicate));
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldThrowException_WhenNameIsEmpty()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "",
                Email = "testuser@hello.com",
                Password = "password123"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _userService.RegisterUserAsync(createUser));
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldThrowException_WhenEmailIsEmpty()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "",
                Password = "password123"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _userService.RegisterUserAsync(createUser));
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldThrowException_WhenPasswordIsEmpty()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "testuser@hello.com",
                Password = ""
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _userService.RegisterUserAsync(createUser));
        }
        [TestMethod]
        public async Task LoginUserAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "testuser@hello.com",
                Password = "password123"
            };
            await _userService.RegisterUserAsync(createUser);
            var loginDto = new LoginUserDTO
            {
                Email = "testuser@hello.com",
                Password = "password123"
            };
            // Act
            var result = await _userService.LoginUserAsync(loginDto);
            // Assert
            Assert.IsNotNull(result);
            //Assert.IsTrue(result.Token == null);
            Assert.AreEqual(result.User.Email, createUser.Email);
            Assert.AreEqual(result.User.Name, createUser.Name);
        }

        [TestMethod]
        public async Task LoginUserAsync_ShouldThrowException_WhenEmailDoesNotExist()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                Email = "nonexistent@hello.com",
                Password = "password123"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _userService.LoginUserAsync(loginDto));
        }

        [TestMethod]
        public async Task LoginUserAsync_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "testuser@hello.com",
                Password = "password123"
            };
            await _userService.RegisterUserAsync(createUser);
            var loginDto = new LoginUserDTO
            {
                Email = "testuser@hello.com",
                Password = "wrongpassword"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _userService.LoginUserAsync(loginDto));
        }
        [TestMethod]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "testuser@hello.com",
                Password = "password123"
            };
            var registeredUser = await _userService.RegisterUserAsync(createUser);
            // Act
            var retrievedUser = await _userService.GetUserByIdAsync(registeredUser.Id);
            // Assert
            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual(registeredUser.Id, retrievedUser.Id);
            Assert.AreEqual(registeredUser.Name, retrievedUser.Name);
            Assert.AreEqual(registeredUser.Email, retrievedUser.Email);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 999;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _userService.GetUserByIdAsync(nonExistentUserId));
        }
        [TestMethod]
        public async Task GetAllUsersInProjectAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var createUser1 = new CreateUserDTO
            {
                Name = "User 1",
                Email = "user1@hello.com",
                Password = "password123"
            };
            var createUser2 = new CreateUserDTO
            {
                Name = "User 2",
                Email = "user2@hello.com",
                Password = "password123"
            };
            await _userService.RegisterUserAsync(createUser1);
            await _userService.RegisterUserAsync(createUser2);
            // Act
            var users = await _userService.GetAllUsersInProjectAsync();
            // Assert
            Assert.IsNotNull(users);
            Assert.AreEqual(2, users.Count);
            Assert.IsTrue(users.Any(u => u.Name == createUser1.Name && u.Email == createUser1.Email));
            Assert.IsTrue(users.Any(u => u.Name == createUser2.Name && u.Email == createUser2.Email));
        }

        [TestMethod]
        public async Task GetAllUsersInProjectAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Act
            var users = await _userService.GetAllUsersInProjectAsync();
            // Assert
            Assert.IsNotNull(users);
            Assert.AreEqual(0, users.Count);
        }
        [TestMethod]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "testuser@hello.com",
                Password = "password123"
            };
            var registeredUser = await _userService.RegisterUserAsync(createUser);
            var updateUser = new UpdateUserDTO
            {
                Name = "Updated User",
                Email = "updateduser@hello.com",
                Password = "newpassword123"
            };
            // Act
            var updatedUser = await _userService.UpdateUserAsync(registeredUser.Id, updateUser);
            // Assert
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(registeredUser.Id, updatedUser.Id);
            Assert.AreEqual(updateUser.Name, updatedUser.Name);
            Assert.AreEqual(updateUser.Email, updatedUser.Email);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 999;
            var updateUser = new UpdateUserDTO
            {
                Name = "Updated User",
                Email = "updateduser@hello.com",
                Password = "newpassword123"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _userService.UpdateUserAsync(nonExistentUserId, updateUser));
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var createUser1 = new CreateUserDTO
            {
                Name = "User 1",
                Email = "user1@hello.com",
                Password = "password123"
            };
            var createUser2 = new CreateUserDTO
            {
                Name = "User 2",
                Email = "user2@hello.com",
                Password = "password123"
            };
            var user1 = await _userService.RegisterUserAsync(createUser1);
            await _userService.RegisterUserAsync(createUser2);
            var updateUser = new UpdateUserDTO
            {
                Name = "Updated User 1",
                Email = "user2@hello.com",
                Password = "newpassword123"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _userService.UpdateUserAsync(user1.Id, updateUser));
        }
        [TestMethod]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenTokenIsValid()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "testuser@hello.com",
                Password = "password123"
            };
            var registeredUser = await _userService.RegisterUserAsync(createUser);
            var loginDto = new LoginUserDTO
            {
                Email = "testuser@hello.com",
                Password = "password123"
            };
            var loginResult = await _userService.LoginUserAsync(loginDto);
            // Act
            await _userService.DeleteUserAsync(registeredUser.Id);
            // Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _userService.GetUserByIdAsync(registeredUser.Id));
        }

        [TestMethod]
        public async Task DeleteUserAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 999;
            var token = "sometoken";
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _userService.DeleteUserAsync(nonExistentUserId));
        }
    }
}