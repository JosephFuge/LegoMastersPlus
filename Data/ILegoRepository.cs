using LegoMastersPlus.Models;

namespace LegoMastersPlus.Data
{
    public interface ILegoRepository
    {
        public IQueryable<Customer> Customers { get; }
        public void AddCustomer(Customer customer);
        public IQueryable<Product> Products { get; }

        public IQueryable<string> PrimaryColors { get; }
        public IQueryable<string> SecondaryColors { get; }
        public void AddProduct(Product product);
        public void UpdateProduct(Product product);
        public IQueryable<Order> Orders { get; }
        public void DeleteProduct(Product product);
        public IQueryable<ProductItemRecommendation> ProductItemRecommendations(int product_ID);
        public void DeleteOrder(Order order);
        public void SaveChanges();

        public IQueryable<Category> Categories { get; }
        void SaveOrder(Order order);
    }
}
