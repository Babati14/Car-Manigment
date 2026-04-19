using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.Admin;

namespace Car_Manigment.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index() => View();


        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .Select(u => new AdminUserViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName
                })
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(AdminUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var cars = await _db.Cars.Where(c => c.OwnerId == id).ToListAsync();
            _db.Cars.RemoveRange(cars);

            await _userManager.DeleteAsync(user);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Cars()
        {
            var cars = await _db.Cars
                .Include(c => c.Owner)
                .Select(c => new AdminCarViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    VinNumber = c.VinNumber,
                    OwnerName = c.OwnerName,
                    OwnerEmail = c.Owner != null ? c.Owner.Email : null
                })
                .ToListAsync();

            return View(cars);
        }

        [HttpGet]
        public async Task<IActionResult> EditCar(int id)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car == null) return NotFound();

            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(Car car)
        {
            if (!ModelState.IsValid) return View(car);

            _db.Cars.Update(car);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Cars));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _db.Cars.FindAsync(id);
            if (car == null) return NotFound();

            _db.Cars.Remove(car);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Cars));
        }

        public async Task<IActionResult> ServiceOrders()
        {
            var orders = await _db.ServiceOrders
                .Include(so => so.Car)
                .Include(so => so.CreatedBy)
                .Select(so => new AdminServiceOrderViewModel
                {
                    Id = so.Id,
                    Description = so.Description,
                    EstimatedPrice = so.EstimatedPrice,
                    Status = so.Status.ToString(),
                    CarDisplayName = so.Car.Brand + " " + so.Car.Model + " (" + so.Car.Year + ")",
                    CreatedByEmail = so.CreatedBy != null ? so.CreatedBy.Email : null,
                    CreatedOn = so.CreatedOn
                })
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> EditServiceOrder(int id)
        {
            var order = await _db.ServiceOrders.FindAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditServiceOrder(ServiceOrder order)
        {
            if (!ModelState.IsValid) return View(order);

            _db.ServiceOrders.Update(order);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ServiceOrders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteServiceOrder(int id)
        {
            var order = await _db.ServiceOrders.FindAsync(id);
            if (order == null) return NotFound();

            _db.ServiceOrders.Remove(order);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ServiceOrders));
        }
    }
}
