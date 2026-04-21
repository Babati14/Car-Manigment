using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.Reports;

namespace Car_Manigment.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public ReportsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchCar)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var userId = user.Id;

            var carsQuery = _db.Cars.Where(c => c.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(searchCar))
            {
                carsQuery = carsQuery.Where(c => c.Brand.Contains(searchCar) || c.Model.Contains(searchCar));
            }

            var cars = await carsQuery
                .Include(c => c.ServiceOrders)
                .ToListAsync();

            var allOrders = cars.SelectMany(c => c.ServiceOrders).ToList();

            var model = new ReportsDashboardViewModel
            {
                TotalCars = cars.Count,
                TotalServiceOrders = allOrders.Count,
                TotalSpent = allOrders.Sum(o => o.EstimatedPrice),

                PendingOrders = allOrders.Count(o => o.Status == ServiceStatus.Pending),
                InProgressOrders = allOrders.Count(o => o.Status == ServiceStatus.InProgress),
                CompletedOrders = allOrders.Count(o => o.Status == ServiceStatus.Completed),

                CarSummaries = cars.Select(c => new CarServiceSummary
                {
                    CarDisplayName = $"{c.Brand} {c.Model} ({c.Year})",
                    ServiceCount = c.ServiceOrders.Count,
                    TotalSpent = c.ServiceOrders.Sum(o => o.EstimatedPrice)
                })
                .OrderByDescending(c => c.TotalSpent)
                .ToList(),

                MonthlySpending = allOrders
                    .GroupBy(o => new { o.CreatedOn.Year, o.CreatedOn.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new MonthlySpending
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Amount = g.Sum(o => o.EstimatedPrice)
                    })
                    .ToList()
            };

            ViewBag.SearchCar = searchCar;
            return View(model);
        }
    }
}
