using GoodHamburger.Models;
using GoodHamburger.Models.Order;
using GoodHamburger.Models.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GoodHamburger
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Sandwich> Sandwich { get; set; }
        public DbSet<Extra> Extra { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
    }
}
