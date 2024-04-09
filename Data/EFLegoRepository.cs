using LegoMastersPlus.Models;
using Microsoft.EntityFrameworkCore;

namespace LegoMastersPlus.Data
{
    public class EFLegoRepository : ILegoRepository
    {
        private LegoMastersDbContext _context;

        public EFLegoRepository(LegoMastersDbContext temp)
        {
            _context = temp;
        }

        public IQueryable<Customer> Customers => _context.Customers.Include("IdentityUser");

        public void AddCustomer(Customer customer)
        {
            _context.Add(customer);
            _context.SaveChanges();
        }

        public IQueryable<Product> Products => _context.Products;

        public void AddProduct(Product product) {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product) {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public IQueryable<Order> Orders => _context.Orders.Include(order => order.LineItems);
    }
}
