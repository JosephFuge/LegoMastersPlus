using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LegoMastersPlus.Models
{
    public class Order
    {
        [Key]
        public int transaction_ID { get; set; }
        public ICollection<LineItem> LineItems { get; set; }
        [Required]
        [ForeignKey("Customer")]
        public int customer_ID { get; set; }
        public Customer Customer { get; set; }
        [Required]
        public string date { get; set; }
        [Required]
        public string day_of_week { get; set; }
        [Required]
        public int time { get; set; }
        [Required]
        public string entry_mode { get; set; }
        [Required]
        public int amount { get; set; }
        [Required]
        public string type_of_transaction { get; set; }
        [Required]
        public string country_of_transaction { get; set; }
        [Required]
        public string shipping_address { get; set; }
        [Required]
        public string bank { get; set; }
        [Required]
        public string type_of_card { get; set; }
        [Required]
        public bool fraud { get; set; }
    }
}
