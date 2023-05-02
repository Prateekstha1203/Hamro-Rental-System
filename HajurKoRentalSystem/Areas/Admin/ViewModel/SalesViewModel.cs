namespace HajurKoRentalSystem.Areas.Admin.ViewModel
{
    public class SalesViewModel
    {
        public List<VehicleRentViewModel> VehicleRentCount { get; set; }

        public List<UserRentViewModel> UserRentCount { get; set; }

       

        public List<InActiveCustomerViewModel> InactiveUserCount { get; set; }
    }
}
