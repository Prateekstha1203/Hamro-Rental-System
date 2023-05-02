namespace HajurKoRentalSystem.Areas.Admin.ViewModel
{
    public class CustomerRentDetails
    {
        public Models.User Customer { get; set; }

        public List<CustomerDetailRentViewModel> CustomerRent { get; set; }
    }
}
