using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Car_Manigment.Controllers;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarManigment.Tests.Controllers
{
    public class ReportsControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly ReportsController _controller;
        private readonly string _testUserId = "test-user-123";
        private readonly string _otherUserId = "other-user-456";

        public ReportsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            _controller = new ReportsController(_context, _mockUserManager.Object);

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
            var car1 = new Car
            {
                Id = 1,
                Brand = "Toyota",
                Model = "Camry",
                Year = 2020,
                VinNumber = "VIN001",
                OwnerName = "Test Owner",
                OwnerPhone = "123",
                OwnerId = _testUserId
            };

            var car2 = new Car
            {
                Id = 2,
                Brand = "Honda",
                Model = "Civic",
                Year = 2021,
                VinNumber = "VIN002",
                OwnerName = "Test Owner",
                OwnerPhone = "123",
                OwnerId = _testUserId
            };

            var car3 = new Car
            {
                Id = 3,
                Brand = "Ford",
                Model = "F-150",
                Year = 2019,
                VinNumber = "VIN003",
                OwnerName = "Other Owner",
                OwnerPhone = "456",
                OwnerId = _otherUserId
            };

            _context.Cars.AddRange(car1, car2, car3);

            var orders = new List<ServiceOrder>
            {
                new ServiceOrder
                {
                    Id = 1,
                    Description = "Oil change",
                    EstimatedPrice = 50.00m,
                    CarId = 1,
                    CreatedById = _testUserId,
                    Status = ServiceStatus.Pending,
                    CreatedOn = new DateTime(2024, 1, 15)
                },
                new ServiceOrder
                {
                    Id = 2,
                    Description = "Tire rotation",
                    EstimatedPrice = 75.00m,
                    CarId = 1,
                    CreatedById = _testUserId,
                    Status = ServiceStatus.Completed,
                    CreatedOn = new DateTime(2024, 2, 10)
                },
                new ServiceOrder
                {
                    Id = 3,
                    Description = "Brake service",
                    EstimatedPrice = 200.00m,
                    CarId = 2,
                    CreatedById = _testUserId,
                    Status = ServiceStatus.InProgress,
                    CreatedOn = new DateTime(2024, 1, 20)
                },
                new ServiceOrder
                {
                    Id = 4,
                    Description = "Engine repair",
                    EstimatedPrice = 500.00m,
                    CarId = 3,
                    CreatedById = _otherUserId,
                    Status = ServiceStatus.Completed,
                    CreatedOn = new DateTime(2024, 1, 5)
                }
            };

            _context.ServiceOrders.AddRange(orders);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_WithAuthenticatedUser_ReturnsDashboardViewModel()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.Equal(2, model.TotalCars);
            Assert.Equal(3, model.TotalServiceOrders);
            Assert.Equal(325.00m, model.TotalSpent);
        }

        [Fact]
        public async Task Index_CountsOrdersByStatus()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.Equal(1, model.PendingOrders);
            Assert.Equal(1, model.InProgressOrders);
            Assert.Equal(1, model.CompletedOrders);
        }

        [Fact]
        public async Task Index_WithSearchParameter_FiltersCarsByBrand()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index("Toyota");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.Equal(1, model.TotalCars);
            Assert.Equal(2, model.TotalServiceOrders);
        }

        [Fact]
        public async Task Index_WithSearchParameter_FiltersCarsByModel()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index("Civic");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.Equal(1, model.TotalCars);
            Assert.Single(model.CarSummaries);
        }

        [Fact]
        public async Task Index_WithInvalidSearchInput_ReturnsViewWithError()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index("<script>alert('xss')</script>");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Index_WhenUserNotAuthenticated_ReturnsChallengeResult()
        {
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((IdentityUser)null);

            var result = await _controller.Index(null);

            Assert.IsType<ChallengeResult>(result);
        }

        [Fact]
        public async Task Index_CarSummaries_OrderedByTotalSpentDescending()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            var summaries = model.CarSummaries.ToList();
            Assert.Equal(2, summaries.Count);
            Assert.True(summaries[0].TotalSpent >= summaries[1].TotalSpent);
        }

        [Fact]
        public async Task Index_CalculatesMonthlySpending()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.NotEmpty(model.MonthlySpending);
        }

        [Fact]
        public async Task Index_OnlyIncludesUserCars()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.DoesNotContain(model.CarSummaries, c => c.CarDisplayName.Contains("Ford"));
        }

        [Fact]
        public async Task Index_CaseInsensitiveSearch()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index("TOYOTA");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.Equal(1, model.TotalCars);
        }

        [Fact]
        public async Task Index_WithEmptySearch_ReturnsAllUserCars()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index("");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.Equal(2, model.TotalCars);
        }

        [Fact]
        public async Task Index_WithWhitespaceSearch_ReturnsAllUserCars()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index("   ");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ReportsDashboardViewModel>(viewResult.Model);
            Assert.Equal(2, model.TotalCars);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
