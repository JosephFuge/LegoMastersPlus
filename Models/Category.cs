using System.ComponentModel.DataAnnotations;

namespace LegoMastersPlus.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}
