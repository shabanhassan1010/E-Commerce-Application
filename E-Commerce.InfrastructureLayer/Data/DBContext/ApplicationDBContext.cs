using E_Commerce.DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.InfrastructureLayer.Data.DBContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> context):base (context)
        {
            
        }
        public DbSet<Product> products { get; set; }
    }
}
