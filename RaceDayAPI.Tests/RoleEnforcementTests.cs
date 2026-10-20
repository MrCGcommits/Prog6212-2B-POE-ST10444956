using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Controllers;
using RaceDayAPI.Data;
using System.Reflection;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class RoleEnforcementTests
    {
        private RaceDayDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        [Fact]
        public void CreateEvent_HasOrganiserOnlyAuthorizeAttribute()
        {
            // Arrange
            var methodInfo = typeof(EventsController).GetMethod("CreateEvent");

            // Act
            var authorizeAttribute = methodInfo?.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Organiser", authorizeAttribute.Roles); // Proves only Organisers can access
        }

        [Fact]
        public async Task CreateEvent_WithoutUserClaim_ReturnsUnauthorized()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EventsController(context);

            // Add an empty HttpContext so the controller doesn't crash when checking User
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            var newEvent = new EventsController.EventDto
            {
                EventName = "Test Event",
                Description = "Test",
                EventDate = DateTime.UtcNow.AddDays(10),
                Location = "Test Location",
                Distance = 10.0m,
                EventType = "run"
            };

            // Act
            var result = await controller.CreateEvent(newEvent);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public void CaptureResult_HasOrganiserOnlyAuthorizeAttribute()
        {
            // Arrange
            var methodInfo = typeof(ResultsController).GetMethod("CaptureResult");

            // Act
            var authorizeAttribute = methodInfo?.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal("Organiser", authorizeAttribute.Roles);
        }
    }
}