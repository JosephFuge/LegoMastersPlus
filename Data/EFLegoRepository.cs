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

        public void UpdateCustomer(Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
        }

        public void DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }

        public IQueryable<Product> Products => _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category);

        public IQueryable<string> PrimaryColors => _context.Products.Select(p => p.primary_color).Distinct();
        public IQueryable<string> SecondaryColors => _context.Products.Select(p => p.secondary_color).Distinct();

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
        public IQueryable<ProductItemRecommendation> ProductItemRecommendations(int product_ID) => _context.ProductItemRecommendations
                                                                                    .Where(pir => pir.product_ID == product_ID)
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
        
        public int SaveOrder(Order order)
        {
            // Add your logic to save the order to the database
            // For example, using Entity Framework:
            _context.Add(order);
            _context.SaveChanges();
            return order.transaction_ID;
        }

        public IQueryable<Category> Categories => _context.Categories;
        
        public IQueryable<Order> Orders => _context.Orders.Include(order => order.LineItems);

        public IQueryable<LineItem> LineItems => _context.LineItems.Include(lineItem => lineItem.Product);



    }
}
