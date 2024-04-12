using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegoMastersPlus.Models
{
    public class ProductUserRecommendation
    {
        [Key]
        public long index {  get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int customer_ID { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey("Product_1")]
        public int Recommendation_1 { get; set; }
        public Product Product_1 { get; set; }
        [ForeignKey("Product_2")]
        public int Recommendation_2 { get; set; }    
        public Product Product_2 { get; set; }
        [ForeignKey("Product_3")]
        public int Recommendation_3 { get; set; }
        public Product Product_3 { get; set; }
        [ForeignKey("Product_4")]
        public int Recommendation_4 { get; set; }
        public Product Product_4 { get; set; }
        [ForeignKey("Product_5")]
        public int Recommendation_5 { get; set; }
        public Product Product_5 { get; set; }
        [ForeignKey("Product_6")]
        public int Recommendation_6 { get; set; }
        public Product Product_6 { get; set; }
        [ForeignKey("Product_7")]
        public int Recommendation_7 { get; set; }
        public Product Product_7 { get; set; }
        [ForeignKey("Product_8")]
        public int Recommendation_8 { get; set; }
        public Product Product_8 { get; set; }
        [ForeignKey("Product_9")]
        public int Recommendation_9 { get; set; }
        public Product Product_9 { get; set; }
        [ForeignKey("Product_10")]
        public int Recommendation_10 { get; set; }
        public Product Product_10 { get; set; }
    }
}
