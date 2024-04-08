using LegoMastersPlus.Models;

namespace LegoMastersPlus.Data
{
    public interface ILegoRepository
    {
        public IQueryable Customers { get; }
        public void AddCustomer(Customer customer);
        public IQueryable Products { get; }
        public IQueryable Orders { get; }
    }
}
