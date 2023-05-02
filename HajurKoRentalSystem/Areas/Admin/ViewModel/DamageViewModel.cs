namespace HajurKoRentalSystem.Areas.Admin.ViewModel
{
    public class DamageViewModel
    {
        public int DamageId { get; set; }

        public string Description { get; set; }

        public string RequestDate { get; set; }

        public string ApprovedDate { get; set; }

        public int RentalId { get; set; }

        public int VehicleId { get; set; }

        public string VehicleName { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }

        public string ApproverId { get; set; }

        public string ApproverName { get; set; }

        public string PaymentStatus { get; set; }

        public string Cost { get; set; }

        public float RepairCost { get; set; }
    }
}
