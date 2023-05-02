using HajurKoRentalSystem.Models;

namespace HajurKoRentalSystem.Areas.Admin.ViewModel
{
    public class VehicleDetailViewModel
    {
        public Vehicle Vehicle { get; set; }


        public List<VehicleDetailRentViewModel> RentailDetails { get; set; }
    }
}
