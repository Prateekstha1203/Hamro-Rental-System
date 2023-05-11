using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurKoRentalSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string? CitizenshipNumber { get; set; }

        public byte[]? Citizenship { get; set; } 

        public string? LicenseNumber { get; set; }

        public byte[]? License { get; set; }

        public string UserId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsRegular { get; set; } = true;

        [ValidateNever]
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ValidateNever]
        public virtual ICollection<Rental>? Rental { get; set; }
    }
}
