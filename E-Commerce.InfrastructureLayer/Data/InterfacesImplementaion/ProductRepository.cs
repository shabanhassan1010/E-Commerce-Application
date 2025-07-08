#region MyRegion
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
#endregion

namespace E_Commerce.InfrastructureLayer.Data.GenericClass
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        #region context
        private readonly ApplicationDBContext context;
        public ProductRepository(ApplicationDBContext context) : base(context)
        {
            this.context = context;
        }
        #endregion
        public async Task<IReadOnlyList<Product>> GetBrandsAsync(string brand)
        {
            return await context.products.AsNoTracking()
                .Where(p => p.Brand.ToLower() == brand.ToLower()).ToListAsync();
        }
        public async Task<IReadOnlyList<Product>> GetTypesAsync(string type)
        {
            return await context.products.AsNoTracking()
                .Where(p => p.Type.ToLower() == type.ToLower()).ToListAsync();

        }
        public async Task<PaginationResponse<Product>> GetProductsPagedAsync(int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;  // always i will display the first page to user
            if (pageSize  < 1) pageSize = 10;  // each page have 10 items so must return 10items for him

            // Get total number of items
            var totalItems = await context.products.AsNoTracking().CountAsync();

            // Fetch paginated data (ordered by Id for consistency)
            var data = await context.products.OrderBy(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<Product>(pageIndex, pageSize, totalItems, data);
        }
        public async Task<IReadOnlyList<Product>> FilterProductsAsync(string? brand, string? type , string? sort)
        {
            var query = context.products.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(b => b.Brand.ToLower() == brand.ToLower());

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(T => T.Type.ToLower() == type.ToLower());

            query = sort switch
            {
                "PriceAsc" => query.OrderBy(x => x.Price),
                "PriceDesc" => query.OrderByDescending(x => x.Price),
                _ => query.OrderBy(x => x.Name)
            };
            return await query.ToListAsync();
        }
        public async Task<IReadOnlyList<Product>> SearchProductsAsync(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await context.products.ToListAsync();

            searchTerm = searchTerm.ToLower();
            return await context.products.AsNoTracking()
                .Where(p =>
                    p.Name.ToLower().StartsWith(searchTerm) ||       
                    p.Description.ToLower().StartsWith(searchTerm) || 
                    p.Brand.ToLower().StartsWith(searchTerm) ||      
                    p.Type.ToLower().StartsWith(searchTerm)).OrderBy(p => p.Name).ToListAsync();
        }
        public async Task<PaginationResponse<Product>> FuzzySearchAsync(string query , int page, int pageSize)
        {
            var products = await context.products.ToListAsync();

            // Apply fuzzy filter on name only
            var fuzzyMatched = products.Where(p=>Fuzz.PartialRatio(p.Name?.ToLower() ?? "", query.ToLower()) > 70).ToList();
            var totalItems = fuzzyMatched.Count;

            var pagedData = fuzzyMatched
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginationResponse<Product>(page, pageSize, totalItems, pagedData);
        }
    }
}
