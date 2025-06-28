using E_Commerce.DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.InfrastructureLayer.Data.Config
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.HasOne(sc => sc.User).WithMany().HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(sc => sc.CartItems).WithOne(ci => ci.ShoppingCart)
                .HasForeignKey(ci => ci.ShoppingCartId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
