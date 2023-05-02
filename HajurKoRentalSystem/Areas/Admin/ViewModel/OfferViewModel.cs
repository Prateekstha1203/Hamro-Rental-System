using HajurKoRentalSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HajurKoRentalSystem.Areas.Admin.ViewModel
{
    public class OfferViewModel
    {
        public Offer Offer { get; set; }

        public List<SelectListItem> Vehicles { get; set; }

        public List<int> VehicleList { get; set; }
    }
}
