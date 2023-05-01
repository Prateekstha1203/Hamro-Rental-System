namespace HajurKoRentalSystem.Areas.Admin.ViewModel
{
	public class RentalDetailsViewModel
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		public string UserName { get; set; }

		public int VehicleId { get; set; }

		public string VehicleName { get; set; }

		public string RequestedDate { get; set; }

		public string StartDate { get; set; }

		public string EndDate { get; set; }

		public string? ActionDate { get; set; }

		public string? ReturnedDate { get; set; }

		public string ActionBy { get; set; }

		public string TotalAmount { get; set; }

		public byte[] VehicleImages { get; set; }
	}
}
