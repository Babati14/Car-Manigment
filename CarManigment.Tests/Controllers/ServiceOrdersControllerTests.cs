using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Car_Manigment.Controllers;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.ServiceOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarManigment.Tests.Controllers
{
    public class ServiceOrdersControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly ServiceOrdersController _controller;
        private readonly string _testUserId = "test-user-123";
        private readonly string _otherUserId = "other-user-456";

        public ServiceOrdersControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            _controller = new ServiceOrdersController(_context, _mockUserManager.Object);

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
                VinNumber = "VIN123",
                OwnerName = "Test Owner",
                OwnerPhone = "1234567890",
                OwnerId = _testUserId
            };

            var car2 = new Car
            {
                Id = 2,
                Brand = "Honda",
                Model = "Accord",
                Year = 2021,
                VinNumber = "VIN456",
                OwnerName = "Other Owner",
                OwnerPhone = "0987654321",
                OwnerId = _otherUserId
            };

            _context.Cars.AddRange(car1, car2);

            var so1 = new ServiceOrder
            {
                Id = 1,
                Description = "Oil change",
                EstimatedPrice = 50.00m,
                CarId = 1,
                CreatedById = _testUserId,
                Status = ServiceStatus.Pending
            };

            var so2 = new ServiceOrder
            {
                Id = 2,
                Description = "Tire rotation",
                EstimatedPrice = 75.00m,
                CarId = 2,
                CreatedById = _otherUserId,
                Status = ServiceStatus.InProgress
            };

            _context.ServiceOrders.AddRange(so1, so2);
            _context.SaveChanges();
        }

        #region Index Tests

        [Fact]
        public async Task Index_ReturnsViewWithUserServiceOrders()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ServiceOrderListViewModel>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("Oil change", model.First().Description);
        }

        [Fact]
        public async Task Index_WhenUserNotFound_ReturnsChallengeResult()
        {
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((IdentityUser)null);

            var result = await _controller.Index();

            Assert.IsType<ChallengeResult>(result);
        }

        [Fact]
        public async Task Index_FiltersServiceOrdersByUserCars()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ServiceOrderListViewModel>>(viewResult.Model);
            Assert.DoesNotContain(model, so => so.Description == "Tire rotation");
        }

        #endregion

        #region Create GET Tests

        [Fact]
        public void Create_ReturnsViewWithUserCars()
        {
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(_testUserId);

            var result = _controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(_controller.ViewBag.Cars);
        }

        #endregion

        #region Create POST Tests

        [Fact]
        public async Task Create_WithValidModel_CreatesServiceOrder()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var model = new ServiceOrderCreateViewModel
            {
                Description = "Brake repair",
                EstimatedPrice = 150.00m,
                CarId = 1
            };

            var result = await _controller.Create(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
            Assert.Equal(3, _context.ServiceOrders.Count());
        }

        [Fact]
        public async Task Create_WithInvalidModel_ReturnsViewWithModel()
        {
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(_testUserId);

            _controller.ModelState.AddModelError("Description", "Required");

            var model = new ServiceOrderCreateViewModel { CarId = 1 };

            var result = await _controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Create_WithInvalidCarOwnership_ReturnsViewWithError()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(_testUserId);

            var model = new ServiceOrderCreateViewModel
            {
                Description = "Test",
                EstimatedPrice = 100m,
                CarId = 2
            };

            var result = await _controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_WithNonExistentCar_ReturnsViewWithError()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(_testUserId);

            var model = new ServiceOrderCreateViewModel
            {
                Description = "Test",
                EstimatedPrice = 100m,
                CarId = 999
            };

            var result = await _controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        #endregion

        #region Edit GET Tests

        [Fact]
        public async Task Edit_WithValidId_ReturnsViewWithModel()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(_testUserId);

            var result = await _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ServiceOrderEditViewModel>(viewResult.Model);
            Assert.Equal("Oil change", model.Description);
        }

        [Fact]
        public async Task Edit_WithNonExistentId_ReturnsNotFound()
        {
            var result = await _controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_WhenUserNotAuthenticated_ReturnsChallengeResult()
        {
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((IdentityUser)null);

            var result = await _controller.Edit(1);

            Assert.IsType<ChallengeResult>(result);
        }

        [Fact]
        public async Task Edit_WhenNotCarOwner_ReturnsForbidResult()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Edit(2);

            Assert.IsType<ForbidResult>(result);
        }

        #endregion

        #region Edit POST Tests

        [Fact]
        public async Task Edit_WithValidModel_UpdatesServiceOrder()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var model = new ServiceOrderEditViewModel
            {
                Id = 1,
                Description = "Updated description",
                EstimatedPrice = 200.00m,
                Status = ServiceStatus.Completed,
                CarId = 1
            };

            var result = await _controller.Edit(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            var updated = await _context.ServiceOrders.FindAsync(1);
            Assert.Equal("Updated description", updated.Description);
            Assert.Equal(200.00m, updated.EstimatedPrice);
        }

        [Fact]
        public async Task Edit_WithInvalidModel_ReturnsViewWithModel()
        {
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(_testUserId);

            _controller.ModelState.AddModelError("Description", "Required");

            var model = new ServiceOrderEditViewModel { Id = 1, CarId = 1 };

            var result = await _controller.Edit(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Edit_WhenServiceOrderNotFound_ReturnsNotFound()
        {
            var model = new ServiceOrderEditViewModel { Id = 999, CarId = 1 };

            var result = await _controller.Edit(model);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_WhenNotCarOwner_ReturnsForbid()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var model = new ServiceOrderEditViewModel
            {
                Id = 2,
                Description = "Test",
                EstimatedPrice = 100m,
                CarId = 2
            };

            var result = await _controller.Edit(model);

            Assert.IsType<ForbidResult>(result);
        }

        #endregion

        #region Delete GET Tests

        [Fact]
        public async Task Delete_WithValidId_ReturnsViewWithModel()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ServiceOrder>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task Delete_WithNonExistentId_ReturnsNotFound()
        {
            var result = await _controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WhenNotCarOwner_ReturnsForbid()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.Delete(2);

            Assert.IsType<ForbidResult>(result);
        }

        #endregion

        #region Delete POST Tests

        [Fact]
        public async Task DeleteConfirmed_WithValidId_DeletesServiceOrder()
        {
            var testUser = new IdentityUser { Id = _testUserId };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);

            var result = await _controller.DeleteConfirmed(1, 1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
            Assert.Null(await _context.ServiceOrders.FindAsync(1));
        }

        [Fact]
        public async Task DeleteConfirmed_WithMismatchedIds_ReturnsBadRequest()
        {
            var result = await _controller.DeleteConfirmed(1, 2);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_WithNonExistentId_RedirectsToIndex()
        {
            var result = await _controller.DeleteConfirmed(999, 999);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
