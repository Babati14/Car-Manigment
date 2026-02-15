using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Car_Manigment.Data;
using Car_Manigment.Models;
using Car_Manigment.ViewModels.ServiceOrders;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Manigment.Controllers
{
    public class ServiceOrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ServiceOrdersController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var orders = await _db.ServiceOrders
                .Include(so => so.Car)
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
            ViewBag.Cars = new SelectList(_db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Model).ToList(), "Id", "VinNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceOrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cars = new SelectList(_db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Model).ToList(), "Id", "VinNumber", model.CarId);
                return View(model);
            }

            var so = new ServiceOrder
            {
                Description = model.Description,
                EstimatedPrice = model.EstimatedPrice,
                CarId = model.CarId
            };

            _db.ServiceOrders.Add(so);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var so = await _db.ServiceOrders.FindAsync(id);
            if (so == null) return NotFound();

            var vm = new ServiceOrderEditViewModel
            {
                Id = so.Id,
                Description = so.Description,
                EstimatedPrice = so.EstimatedPrice,
                Status = so.Status,
                CarId = so.CarId
            };

            ViewBag.Cars = new SelectList(_db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Model).ToList(), "Id", "VinNumber", vm.CarId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceOrderEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cars = new SelectList(_db.Cars.OrderBy(c => c.Brand).ThenBy(c => c.Model).ToList(), "Id", "VinNumber", model.CarId);
                return View(model);
            }

            var so = await _db.ServiceOrders.FindAsync(model.Id);
            if (so == null) return NotFound();

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
            return View(so);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var so = await _db.ServiceOrders.FindAsync(id);
            if (so != null)
            {
                _db.ServiceOrders.Remove(so);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}