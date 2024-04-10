using LegoMastersPlus.Models;

namespace LegoMastersPlus.Data
{
    public interface ILegoRepository
    {
        public IQueryable<Customer> Customers { get; }
        public void AddCustomer(Customer customer);
        public IQueryable<Product> Products { get; }
        public void AddProduct(Product product);
        public void UpdateProduct(Product product);
        public IQueryable<Order> Orders { get; }
        public void DeleteProduct(Product product);
        public IQueryable<ProductItemRecommendation> ProductItemRecommendations(int productId);
        public void SaveChanges();
    }
}
