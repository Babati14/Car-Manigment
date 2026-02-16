using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.Cars;
using Car_Manigment.ViewModels.ServiceOrders;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Identity;

namespace Car_Manigment.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CarsController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public CarsController(ApplicationDbContext db, ILogger<CarsController> logger, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var userId = user.Id;

            var cars = await _db.Cars
                .Where(c => c.OwnerId == userId)
                .OrderByDescending(c => c.Id)
                .Select(c => new CarListViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year
                })
                .ToListAsync();

            

            ViewBag.CarsCount = cars.Count;
            return View(cars);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarCreateViewModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            // Prevent duplicate VIN entries
            var existing = await _db.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.VinNumber == inputModel.VinNumber);

            if (existing != null)
            {
                ModelState.AddModelError(nameof(inputModel.VinNumber), "A car with this VIN already exists.");
                return View(inputModel);
            }

            var car = new Car
            {
                Brand = inputModel.Brand,
                Model = inputModel.Model,
                Year = inputModel.Year,
                VinNumber = inputModel.VinNumber,
                OwnerName = inputModel.OwnerName,
                OwnerPhone = inputModel.OwnerPhone
            };

            // If user is authenticated, set OwnerId so the car appears in My Cars
            if (User?.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    car.OwnerId = currentUser.Id;
                }
            }

            try
            {
                _db.Cars.Add(car);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Car added successfully (Id: {car.Id}).";
                return RedirectToAction(nameof(Details), new { id = car.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving car");
                ModelState.AddModelError("", "An unexpected error occurred while saving. See logs for details.");
                return View(inputModel);
            }
        }

        // Removed public All() to prevent listing all cars

        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var userId = currentUser.Id;

            var car = await _db.Cars
                .Include(c => c.ServiceOrders)
                .Where(c => c.Id == id && c.OwnerId == userId)
                .Select(c => new CarDetailsViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    VinNumber = c.VinNumber,
                    OwnerName = c.OwnerName,
                    OwnerPhone = c.OwnerPhone,
                    ServiceOrders = c.ServiceOrders.Select(so => new ServiceOrderListViewModel
                    {
                        Id = so.Id,
                        Description = so.Description,
                        EstimatedPrice = so.EstimatedPrice,
                        CreatedOn = so.CreatedOn,
                        Status = so.Status.ToString(),
                        CarId = so.CarId,
                        CarDisplay = c.Brand + " " + c.Model
                    })
                })
                .FirstOrDefaultAsync();

            if (car == null) return NotFound();
            return View(car);
        }
    }
}