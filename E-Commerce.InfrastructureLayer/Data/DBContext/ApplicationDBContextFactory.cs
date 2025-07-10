using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace E_Commerce.InfrastructureLayer.Data.DBContext
{
    public class ApplicationDBContextFactory : IDesignTimeDbContextFactory<ApplicationDBContext>
    {
        public ApplicationDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();
            optionsBuilder.UseSqlServer("Server=.; Database=E-Commerce_Website ;Integrated Security=True; Encrypt=True ;TrustServerCertificate=True;");

            return new ApplicationDBContext(optionsBuilder.Options);
        }
    }
}
