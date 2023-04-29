using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurKoRentalSystem.Models
{
    public class Staff
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [ValidateNever]
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
