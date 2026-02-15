namespace Car_Manigment.ViewModels.Cars
{
    public class CarListViewModel
    {
        public int Id { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }

        public string DisplayName => $"{Brand} {Model} ({Year})";
    }
}