using System.Collections.Generic;
using Car_Manigment.ViewModels.ServiceOrders;

namespace Car_Manigment.ViewModels.Cars
{
    public class CarDetailsViewModel
    {
        public int Id { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string VinNumber { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string OwnerPhone { get; set; } = null!;

        public IEnumerable<ServiceOrderListViewModel> ServiceOrders { get; set; } = new List<ServiceOrderListViewModel>();
    }
}