using Xunit;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace RaceDayAPI.Tests
{
    public class AuthTests
    {
        // Helper method to create a fresh in-memory database for each test
        private RaceDayDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsCreated()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new AuthController(context);
            var registerDto = new AuthController.RegisterDto
            {
                Email = "test@test.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                Role = "Participant"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsConflict()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new AuthController(context);
            var registerDto = new AuthController.RegisterDto
            {
                Email = "test@test.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                Role = "Participant"
            };

            await controller.Register(registerDto); // Register first time

            // Act
            var result = await controller.Register(registerDto); // Register second time

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new AuthController(context);

            // Manually add a user with a hashed password
            context.Users.Add(new Models.User
            {
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
                Role = "Participant"
            });
            await context.SaveChangesAsync();

            var loginDto = new AuthController.LoginDto
            {
                Email = "test@test.com",
                Password = "WrongPassword!" // Wrong password
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
