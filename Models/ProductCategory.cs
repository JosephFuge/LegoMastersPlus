using Microsoft.EntityFrameworkCore;
//using Mono.TextTemplating;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegoMastersPlus.Models
{
    [PrimaryKey(nameof(product_ID), nameof(CategoryId))]
    public class ProductCategory
    {

        [ForeignKey("Product")]
        public int product_ID { get; set; }
        public Product Product { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
