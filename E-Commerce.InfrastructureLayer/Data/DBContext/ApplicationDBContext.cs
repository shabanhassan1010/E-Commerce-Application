using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer.Data.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.InfrastructureLayer.Data.DBContext
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> context):base (context)
        {
            
        }
        public DbSet<Product> products { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<CartItem> cartItems { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ShoppingCartConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemsConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());

            // Make Tables in Database more redable
            var schema = "Security";

            modelBuilder.Entity<User>().ToTable("Users", schema);
            modelBuilder.Entity<IdentityRole>().ToTable("Roles", schema);
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", schema);
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", schema);
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", schema);
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", schema);
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", schema);
        }
    }
}
