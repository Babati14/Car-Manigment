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

        public async Task<IActionResult> Delete(int id)
        {
            var car = await _db.Cars
                .Include(c => c.ServiceOrders)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (car.OwnerId != currentUser.Id) return Forbid();

            var vm = new CarDetailsViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                VinNumber = car.VinNumber,
                OwnerName = car.OwnerName,
                OwnerPhone = car.OwnerPhone,
                ServiceOrders = car.ServiceOrders.Select(so => new ServiceOrderListViewModel
                {
                    Id = so.Id,
                    Description = so.Description,
                    EstimatedPrice = so.EstimatedPrice,
                    CreatedOn = so.CreatedOn,
                    Status = so.Status.ToString(),
                    CarId = so.CarId,
                    CarDisplay = car.Brand + " " + car.Model
                })
            };

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _db.Cars
                .Include(c => c.ServiceOrders)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return RedirectToAction(nameof(Index));

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (car.OwnerId != currentUser.Id) return Forbid();

            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Car deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (car.OwnerId != currentUser.Id) return Forbid();

            var vm = new CarEditViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                VinNumber = car.VinNumber,
                OwnerName = car.OwnerName,
                OwnerPhone = car.OwnerPhone
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarEditViewModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit Car model state invalid: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(inputModel);
            }

            var car = await _db.Cars.FindAsync(inputModel.Id);
            if (car == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (car.OwnerId != currentUser.Id) return Forbid();

            var existing = await _db.Cars.AsNoTracking().FirstOrDefaultAsync(c => c.VinNumber == inputModel.VinNumber && c.Id != inputModel.Id);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(inputModel.VinNumber), "A car with this VIN already exists.");
                return View(inputModel);
            }

            car.Brand = inputModel.Brand;
            car.Model = inputModel.Model;
            car.Year = inputModel.Year;
            car.VinNumber = inputModel.VinNumber;
            car.OwnerName = inputModel.OwnerName;
            car.OwnerPhone = inputModel.OwnerPhone;

            try
            {
                _db.Cars.Update(car);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Car updated successfully.";
                return RedirectToAction(nameof(Details), new { id = car.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car {CarId}", car.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the car. See logs for details.");
                return View(inputModel);
            }
        }
    }
}