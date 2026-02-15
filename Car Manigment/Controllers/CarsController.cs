using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.Cars;
using Car_Manigment.ViewModels.ServiceOrders;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Manigment.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CarsController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var cars = await _db.Cars
                .Select(c => new CarListViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year
                })
                .ToListAsync();

            return View(cars);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var car = new Car
            {
                Brand = model.Brand,
                Model = model.Model,
                Year = model.Year,
                VinNumber = model.VinNumber,
                OwnerName = model.OwnerName,
                OwnerPhone = model.OwnerPhone
            };

            _db.Cars.Add(car);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _db.Cars
                .Include(c => c.ServiceOrders)
                .Where(c => c.Id == id)
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