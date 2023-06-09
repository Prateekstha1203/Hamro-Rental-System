﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        [Display(Name = "Repair Cost")]
        public float RepairCost { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string DamageDescription { get; set; }

        [Required]
        public bool IsPaid { get; set; } = false;

        public string? PaymentStatus { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public DateTime? ApprovedDate { get; set; }

        [Required]
        public bool IsApproved { get; set; } = false;

        [ValidateNever]
        [ForeignKey("RentalId")]
        public Rental? Rental { get; set; }

        [ValidateNever]
        [ForeignKey("ApprovedBy")]
        public User? User { get; set; }
    }
}
