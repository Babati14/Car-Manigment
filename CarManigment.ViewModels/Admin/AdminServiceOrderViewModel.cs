namespace Car_Manigment.ViewModels.Admin
{
    public class AdminServiceOrderViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public decimal EstimatedPrice { get; set; }
        public string Status { get; set; } = null!;
        public string CarDisplayName { get; set; } = null!;
        public string? CreatedByEmail { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
