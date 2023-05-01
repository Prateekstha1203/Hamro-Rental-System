﻿namespace HajurKoRentalSystem.Areas.User.ViewModels
{
    public class RentalViewModel
    {
        public int VehicleId { get; set; }

        public string VehicleName { get; set; }

        public string VehicleDescription { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerLicenseNumber { get; set; }

        public string CustomerCitizenshipNumber { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double ActualPrice { get; set; }

        public double PriceForRegularAndStaffs { get; set; }
    }
}
