using LegoMastersPlus.Models;

namespace LegoMastersPlus.Data
{
    public interface ILegoRepository
    {
        public IQueryable<Customer> Customers { get; }
        public void AddCustomer(Customer customer);
        public void UpdateCustomer(Customer customer);
        public void DeleteCustomer(Customer customer);
        public IQueryable<Product> Products { get; }

        public IQueryable<string> PrimaryColors { get; }
        public IQueryable<string> SecondaryColors { get; }
        public void AddProduct(Product product);
        public void UpdateProduct(Product product);
        public IQueryable<Order> Orders { get; }
        public void DeleteProduct(Product product);
        public IQueryable<ProductItemRecommendation> ProductItemRecommendations(int product_ID);
        public IQueryable<ProductUserRecommendation> ProductUserRecommendations(int customer_ID);
        public void DeleteOrder(Order order);
        public void SaveChanges();

        public IQueryable<Category> Categories { get; }
        public int SaveOrder(Order order);

        public IQueryable<LineItem> LineItems { get; }

    }
}
