namespace HajurKoRentalSystem.Areas.Admin.ViewModel
{
    public class VehicleRentViewModel
    {
        public string Vehicle { get; set; }

        public int Count { get; set; }
    }

    public class UserRentViewModel
    {
        public string User { get; set; }

        public int Count { get; set; }
    }

    public class InActiveCustomerViewModel
    {
        public string UserId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string LastRentedDate { get; set; }
    }
}
