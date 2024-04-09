﻿using LegoMastersPlus.Models;
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

        public IQueryable Customers => _context.Customers.Include("IdentityUser");

        public void AddCustomer(Customer customer)
        {
            _context.Add(customer);
            _context.SaveChanges();
        }

        public IQueryable Products => _context.Products;

        public IQueryable Orders => _context.Orders.Include(order => order.LineItems);
    }
}