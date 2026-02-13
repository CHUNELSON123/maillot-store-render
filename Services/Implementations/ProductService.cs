using MaillotStore.Data;
using MaillotStore.Models;
using MaillotStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaillotStore.Services.Implementations
{
    public class ProductService : IProductService
    {
        // --- FIX: Use Factory to ensure fresh data every time ---
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public ProductService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.Products
                .AsNoTracking()
                .Include(p => p.Team)
                .Include(p => p.League)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using var context = _factory.CreateDbContext();
            return await context.Products
                .AsNoTracking()
                .Include(p => p.Team)
                .Include(p => p.League)
                .Include(p => p.Gallery) // Necessary for Details page
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.Products
                .AsNoTracking()
                .Include(p => p.Team)
                .Include(p => p.League)
                .Where(p => p.IsFeatured)
                .OrderByDescending(p => p.ProductId)
                .Take(18) // Updated to 18 to match your Home Page layout
                .ToListAsync();
        }

        public async Task<List<Product>> GetOnSaleProductsAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.Products
                .AsNoTracking()
                .Include(p => p.Team)
                .Include(p => p.League)
                .Where(p => p.IsOnSale)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            using var context = _factory.CreateDbContext();
            return await context.Products
                .AsNoTracking()
                .Include(p => p.Team)
                .Include(p => p.League)
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByTeamIdAsync(int teamId)
        {
            using var context = _factory.CreateDbContext();
            return await context.Products
                .AsNoTracking()
                .Include(p => p.Team)
                .Include(p => p.League)
                .Where(p => p.TeamId == teamId)
                .ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            using var context = _factory.CreateDbContext();
            context.Products.Add(product);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            using var context = _factory.CreateDbContext();
            context.Products.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            using var context = _factory.CreateDbContext();
            var product = await context.Products.FindAsync(id);
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<Product>();

            using var context = _factory.CreateDbContext();
            return await context.Products
                .AsNoTracking()
                .Include(p => p.Team)
                .Include(p => p.League)
                .Where(p => p.Name.ToLower().Contains(query.ToLower()) ||
                            p.Description.ToLower().Contains(query.ToLower()) ||
                            (p.Team != null && p.Team.Name.ToLower().Contains(query.ToLower())))
                .ToListAsync();
        }
    }
}