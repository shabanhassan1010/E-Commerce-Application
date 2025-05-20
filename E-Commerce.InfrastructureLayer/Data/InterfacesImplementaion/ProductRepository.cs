#region MyRegion
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using Microsoft.EntityFrameworkCore;
#endregion

namespace E_Commerce.InfrastructureLayer.Data.GenericClass
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        private readonly ApplicationDBContext context;
        public ProductRepository(ApplicationDBContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IReadOnlyList<string>> GetBrandsAsync()
        {
            return await context.products.Select(B => B.Brand)
                    .Distinct().ToListAsync();
        }
        public async Task<IReadOnlyList<string>> GetTypesAsync()
        {
            return await context.products.Select(t => t.Type)
                .Distinct().ToListAsync();
        }
        public async Task<PaginationResponse<Product>> GetProductsPagedAsync(int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;  // always i will display the first page to user
            if (pageSize  < 1) pageSize = 10;  // each page have 10 items so must return 10items for him

            // Get total number of items
            var totalItems = await context.products.CountAsync();

            // Fetch paginated data (ordered by Id for consistency)
            var data = await context.products.OrderBy(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<Product>(pageIndex, pageSize, totalItems, data);
        }
        public async Task<IReadOnlyList<Product>> FilterProductByBrand(string? brand, string? type , string? sort)
        {
            var query = context.products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(b => b.Brand == brand);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(T => T.Type == type);

            query = sort switch
            {
                "PriceAse" => query.OrderBy(x => x.Price),
                "PriceDesc" => query.OrderByDescending(x => x.Price),
                _ => query.OrderBy(x => x.Name)
            };
            return await query.ToListAsync();
        }
    }
}
