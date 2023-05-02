namespace HajurKoRentalSystem.Areas.User.ViewModels
{
    public class RecordViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int VehicleId { get; set; }

        public string VehicleName { get; set; }

        public string RequestedDate { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string? ActionDate { get; set; }

        public string? ReturnedDate { get; set; }

        public string ActionBy { get; set; }

        public string TotalAmount { get; set; }

        public byte[] Image { get; set; }

        public string ImageURL { get; set; }

        public bool IsDamaged { get; set; }
    }
}
