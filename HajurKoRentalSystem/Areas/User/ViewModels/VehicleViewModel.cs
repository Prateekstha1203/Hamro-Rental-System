namespace HajurKoRentalSystem.Areas.User.ViewModels
{
	public class VehicleViewModel
	{
		public int Id { get; set; }
		
		public byte[] Image { get; set; }
		
		public string Name {get; set;}
        
		public float Price {get; set;}
        
		public string Offer { get; set; }
	}
}
