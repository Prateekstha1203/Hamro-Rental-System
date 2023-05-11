namespace HajurKoRentalSystem.Areas.User.ViewModels
{
    public class VehiclesViewModel
    {
        public int Id { get; set; }

        public string ImageURL { get; set; }

        public string Name { get; set; }

        public string? Offer { get; set; }

        public string PricePerDay { get; set; }
    }
}
