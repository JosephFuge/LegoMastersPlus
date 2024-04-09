using LegoMastersPlus.Models;

namespace LegoMastersPlus.Data
{
    public interface ILegoRepository
    {
        public IQueryable<Customer> Customers { get; }
        public void AddCustomer(Customer customer);
        public IQueryable<Product> Products { get; }
        public IQueryable<Order> Orders { get; }
    }
}
