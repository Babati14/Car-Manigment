using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.ServiceOrders;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Car_Manigment.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ServiceOrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public ServiceOrdersController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var userId = currentUser.Id;

            var orders = await _db.ServiceOrders
                .Include(so => so.Car)
                .Where(so => so.Car.OwnerId == userId)
                .Select(so => new ServiceOrderListViewModel
                {
                    Id = so.Id,
                    Description = so.Description,
                    EstimatedPrice = so.EstimatedPrice,
                    CreatedOn = so.CreatedOn,
                    Status = so.Status.ToString(),
                    CarId = so.CarId,
                    CarDisplay = so.Car.Brand + " " + so.Car.Model
                })
                .ToListAsync();

            return View(orders);
        }

        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            // only show cars that belong to current user
            var cars = _db.Cars
                .Where(c => c.OwnerId == userId)
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Select(c => new { c.Id, Display = c.Id + " - " + c.Brand + " " + c.Model })
                .ToList();
            ViewBag.Cars = new SelectList(cars, "Id", "Display");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceOrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cars = _db.Cars
                    .Where(c => c.OwnerId == userId)
                    .OrderBy(c => c.Brand)
                    .ThenBy(c => c.Model)
                    .Select(c => new { c.Id, Display = c.Id + " - " + c.Brand + " " + c.Model })
                    .ToList();
                ViewBag.Cars = new SelectList(cars, "Id", "Display", model.CarId);
                return View(model);
            }

            // ensure the selected car belongs to the current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var car = await _db.Cars.FindAsync(model.CarId);
            if (car == null || car.OwnerId != currentUser.Id)
            {
                ModelState.AddModelError(nameof(model.CarId), "Invalid car selection.");
                var cars = _db.Cars
                    .Where(c => c.OwnerId == _userManager.GetUserId(User))
                    .OrderBy(c => c.Brand)
                    .ThenBy(c => c.Model)
                    .Select(c => new { c.Id, Display = c.Id + " - " + c.Brand + " " + c.Model })
                    .ToList();
                ViewBag.Cars = new SelectList(cars, "Id", "Display", model.CarId);
                return View(model);
            }

            var so = new ServiceOrder
            {
                Description = model.Description,
                EstimatedPrice = model.EstimatedPrice,
                CarId = model.CarId
            };

            if (currentUser != null)
            {
                so.CreatedById = currentUser.Id;
            }

            _db.ServiceOrders.Add(so);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var so = await _db.ServiceOrders.Include(s => s.Car).FirstOrDefaultAsync(s => s.Id == id);
            if (so == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (so.Car.OwnerId != currentUser.Id) return Forbid();

            var vm = new ServiceOrderEditViewModel
            {
                Id = so.Id,
                Description = so.Description,
                EstimatedPrice = so.EstimatedPrice,
                Status = so.Status,
                CarId = so.CarId
            };

            var carsForEdit = _db.Cars
                .Where(c => c.OwnerId == _userManager.GetUserId(User))
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .Select(c => new { c.Id, Display = c.Id + " - " + c.Brand + " " + c.Model })
                .ToList();
            ViewBag.Cars = new SelectList(carsForEdit, "Id", "Display", vm.CarId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceOrderEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cars = _db.Cars
                    .Where(c => c.OwnerId == userId)
                    .OrderBy(c => c.Brand)
                    .ThenBy(c => c.Model)
                    .Select(c => new { c.Id, Display = c.Id + " - " + c.Brand + " " + c.Model })
                    .ToList();
                ViewBag.Cars = new SelectList(cars, "Id", "Display", model.CarId);
                return View(model);
            }

            var so = await _db.ServiceOrders.Include(s => s.Car).FirstOrDefaultAsync(s => s.Id == model.Id);
            if (so == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (so.Car.OwnerId != currentUser.Id) return Forbid();

            so.Description = model.Description;
            so.EstimatedPrice = model.EstimatedPrice;
            so.Status = model.Status;
            so.CarId = model.CarId;

            _db.ServiceOrders.Update(so);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var so = await _db.ServiceOrders.Include(s => s.Car).FirstOrDefaultAsync(s => s.Id == id);
            if (so == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (so.Car.OwnerId != user.Id) return Forbid();

            return View(so);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var so = await _db.ServiceOrders.Include(s => s.Car).FirstOrDefaultAsync(s => s.Id == id);
            if (so != null)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return Challenge();

                if (so.Car.OwnerId != currentUser.Id) return Forbid();

                _db.ServiceOrders.Remove(so);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}