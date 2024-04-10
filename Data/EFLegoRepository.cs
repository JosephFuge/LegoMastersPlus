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
        
        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }

        // Get item recommendations for a specific product and all the products of the associated recommendations
        public IQueryable<ProductItemRecommendation> ProductItemRecommendations(int productId) => _context.ProductItemRecommendations
                                                                                    .Where(pir => pir.product_ID == productId)
                                                                                    .Include(x => x.Product_1)
                                                                                    .Include(x => x.Product_2)
                                                                                    .Include(x => x.Product_3)
                                                                                    .Include(x => x.Product_4)
                                                                                    .Include(x => x.Product_5)
                                                                                    .Include(x => x.Product_6)
                                                                                    .Include(x => x.Product_7)
                                                                                    .Include(x => x.Product_8)
                                                                                    .Include(x => x.Product_9)
                                                                                    .Include(x => x.Product_10)
                                                                                    .Include(pir => pir.RecProduct)
                                                                                        .ThenInclude(pr => pr.ProductCategories)
                                                                                            .ThenInclude(c => c.Category);
        
        public void DeleteOrder(Order order)
        {
            _context.Orders.Remove(order);
        }
        

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        
        public IQueryable<Order> Orders => _context.Orders.Include(order => order.LineItems);
    }
}
