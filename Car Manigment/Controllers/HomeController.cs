using System.Diagnostics;
using Car_Manigment.ViewModels;
using Car_Manigment.ViewModels.Cars;
using Car_Manigment.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Car_Manigment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View(Enumerable.Empty<CarListViewModel>());
            }

            var userId = user.Id;
            var cars = await _db.Cars
                .Where(c => c.OwnerId == userId)
                .OrderByDescending(c => c.Id)
                .Select(c => new CarListViewModel { Id = c.Id, Brand = c.Brand, Model = c.Model, Year = c.Year })
                .ToListAsync();

            return View(cars);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
