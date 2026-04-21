using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Car_Manigment.Controllers;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarManigment.Tests.Controllers
{
    public class HomeControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly HomeController _controller;
        private readonly string _testUserId = "test-user-123";

        public HomeControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            _controller = new HomeController(_context, _mockUserManager.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId)
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            SeedTestData();
        }

        private void SeedTestData()
        {
            var cars = new List<Car>
            {
                new Car
                {
                    Id = 1,
                    Brand = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    VinNumber = "VIN001",
                    OwnerName = "Test Owner",
                    OwnerPhone = "123",
                    OwnerId = _testUserId
                },
                new Car
                {
                    Id = 2,
                    Brand = "Honda",
                    Model = "Civic",
                    Year = 2021,
                    VinNumber = "VIN002",
                    OwnerName = "Test Owner",
                    OwnerPhone = "123",
                    OwnerId = _testUserId
                },
                new Car
                {
                    Id = 3,
                    Brand = "Ford",
                    Model = "F-150",
                    Year = 2019,
                    VinNumber = "VIN003",
                    OwnerName = "Other Owner",
                    OwnerPhone = "456",
                    OwnerId = "other-user-id"
                }
            };

            _context.Cars.AddRange(cars);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_WithAuthenticatedUser_ReturnsViewWithUserCars()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CarListViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
            Assert.All(model, car => Assert.Contains(car.Brand, new[] { "Toyota", "Honda" }));
        }

        [Fact]
        public async Task Index_WithNullUser_ReturnsViewWithEmptyList()
        {
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((IdentityUser)null);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CarListViewModel>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Index_OrdersCarsDescendingById()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CarListViewModel>>(viewResult.Model);
            var modelList = model.ToList();
            Assert.Equal(2, modelList[0].Id); // Honda (Id=2) comes first
            Assert.Equal(1, modelList[1].Id); // Toyota (Id=1) comes second
        }

        [Fact]
        public async Task Index_OnlyReturnsAuthenticatedUserCars()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CarListViewModel>>(viewResult.Model);
            Assert.DoesNotContain(model, car => car.Brand == "Ford");
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            var result = _controller.Privacy();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            var result = _controller.Error();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Car_Manigment.ViewModels.ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
        }

        [Fact]
        public void Error_HasResponseCacheAttribute()
        {
            var methodInfo = typeof(HomeController).GetMethod("Error");

            var attributes = methodInfo?.GetCustomAttributes(typeof(ResponseCacheAttribute), false);

            Assert.NotNull(attributes);
            Assert.Single(attributes);
            var cacheAttribute = attributes[0] as ResponseCacheAttribute;
            Assert.NotNull(cacheAttribute);
            Assert.Equal(0, cacheAttribute.Duration);
            Assert.False(cacheAttribute.NoStore == false);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
