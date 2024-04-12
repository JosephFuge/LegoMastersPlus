using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegoMastersPlus.Models
{
    [PrimaryKey("transaction_ID", "product_ID")]
    public class LineItem
    {
        
        [ForeignKey("Order")]
        public int transaction_ID { get; set; }
        public virtual Order? Order { get; set; }
       
        [ForeignKey("Product")]
        public int product_ID { get; set; }
        public virtual Product? Product { get; set; }
        [Required]
        public int qty { get; set; }
        public int? rating { get; set; }
    }
}
