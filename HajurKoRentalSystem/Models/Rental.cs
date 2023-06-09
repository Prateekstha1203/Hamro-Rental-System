﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurKoRentalSystem.Models
{
    public class Rental
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public string CustomerId { get; set; }

        public DateTime RequestedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public DateTime? ProcessDate { get; set; }

        public bool IsApproved { get; set; } = false;

        public bool IsReturned { get; set; } = false;

        public bool IsCancelled { get; set; } = false;

        public string RentalStatus { get; set; } = Constants.Constants.Pending;

        public string PaymentStatus { get; set; } = Constants.Constants.Pending;

		public string? ApprovedBy { get; set; }

        public bool IsDamaged { get; set; } = false;

        [Required]
        public float TotalAmount { get; set; }

        [ValidateNever]
        [ForeignKey("CustomerId")]
        public User? Customer { get; set; }

        [ValidateNever]
        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }

        [ValidateNever]
        [ForeignKey("ApprovedBy")]
        public User? User { get; set; }
    }
}
