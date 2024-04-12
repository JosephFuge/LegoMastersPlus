namespace LegoMastersPlus.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public int product_ID { get; set; }
        public Product RecProduct { get; set; }
        public List<Product>? StaticRecommendations { get; set; }

        public ProductItemRecommendation? Recommendation { get; set;}
    }
}
