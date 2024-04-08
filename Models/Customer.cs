using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegoMastersPlus.Models
{
    public class Customer
    {
        [Key]
        public int customer_ID { get; set; }
        [ForeignKey("IdentityUser")]
        public string? IdentityID { get; set; }
        public IdentityUser? IdentityUser { get; set; }
        [Required]
        public string first_name { get; set; }
        [Required]
        public string last_name { get; set; }
        [Required]
        public DateOnly birth_date { get; set; }
        [Required]
        public string country_of_residence { get; set; }
        [Required]
        public string gender { get; set; }
        [Required]
        public double age { get; set; }
    }
}
