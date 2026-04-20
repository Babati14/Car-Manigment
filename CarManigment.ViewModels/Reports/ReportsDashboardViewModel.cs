namespace Car_Manigment.ViewModels.Reports
{
    public class ReportsDashboardViewModel
    {
        public int TotalCars { get; set; }
        public int TotalServiceOrders { get; set; }
        public decimal TotalSpent { get; set; }

        public int PendingOrders { get; set; }
        public int InProgressOrders { get; set; }
        public int CompletedOrders { get; set; }

        public List<CarServiceSummary> CarSummaries { get; set; } = new();
        public List<MonthlySpending> MonthlySpending { get; set; } = new();
    }

    public class CarServiceSummary
    {
        public string CarDisplayName { get; set; } = null!;
        public int ServiceCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class MonthlySpending
    {
        public string Month { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
