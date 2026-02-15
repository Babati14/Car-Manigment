using System;

namespace Car_Manigment.ViewModels.ServiceOrders
{
    public class ServiceOrderListViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public decimal EstimatedPrice { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; } = null!;
        public int CarId { get; set; }
        public string CarDisplay { get; set; } = null!;
    }
}