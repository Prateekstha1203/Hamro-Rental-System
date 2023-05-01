using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HajurKoRentalSystem.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Model { get; set; }

        [Required]
        [MaxLength(50)]
        public string Color { get; set; }

        [Required]
        public float PricePerDay { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public string Brand { get; set; }

        public byte[] Image { get; set; }

        public int? OfferId { get; set; }

        public bool IsAvailable { get; set; } = true;

        [ForeignKey("OfferId")]
        public Offer? Offer { get; set; }

        [ValidateNever]
        public virtual ICollection<Rental>? Rental { get; set; }
    }
}
