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
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<CartItem> cartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired();
                entity.HasMany(e => e.cartItems)
                      .WithOne()
                      .HasForeignKey("ShoppingCartId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.ShoppingCartId });
                entity.Property(e => e.ProductName).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.PictureUrl).IsRequired();
            });
        }
    }
}
