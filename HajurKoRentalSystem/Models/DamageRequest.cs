using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurKoRentalSystem.Models
{
    public class DamageRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RentalId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Repair Cost")]
        public decimal RepairCost { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string DamageDescription { get; set; }

        [Required]
        public bool IsPaid { get; set; } = false;

        public string? PaymentStatus { get; set; }

        public string? ApprovedBy { get; set; }

        [Required]
        public bool IsApproved { get; set; } = false;

        public byte[] ImageURL { get; set; }

        [ValidateNever]
        [ForeignKey("RentalId")]
        public Rental? Rental { get; set; }

        [ValidateNever]
        [ForeignKey("ApprovedBy")]
        public User? User { get; set; }
    }
}
