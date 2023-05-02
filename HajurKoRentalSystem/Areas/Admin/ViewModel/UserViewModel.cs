namespace HajurKoRentalSystem.Areas.Admin.ViewModel
{
    public class UserViewModel
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }


        public string Role { get; set; }

        public int TotalRents { get; set; }

        public string LastRentedDate { get; set; }

    }
}
