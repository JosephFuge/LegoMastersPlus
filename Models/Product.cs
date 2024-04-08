using System.ComponentModel.DataAnnotations;

namespace LegoMastersPlus.Models
{
    public class Product
    {
        [Key]
        public int product_ID { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public int year { get; set; }
        [Required]
        public int num_parts { get; set; }
        [Required]
        public decimal price { get; set; }
        [Required]
        public string img_link { get; set; }
        [Required]
        public string primary_color { get; set; }
        [Required]
        public string secondary_color { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string category { get; set; }    
    }
}
