namespace Car_Manigment.ViewModels.Admin
{
    public class AdminCarViewModel
    {
        public int Id { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string VinNumber { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string? OwnerEmail { get; set; }
    }
}
