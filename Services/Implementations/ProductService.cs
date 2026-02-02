using MaillotStore.Data;
using MaillotStore.Models;
using MaillotStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaillotStore.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.Include(p => p.Team).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products.Include(p => p.Team).Take(6).ToListAsync();
        }

        public async Task<List<Product>> GetOnSaleProductsAsync()
        {
            return await _context.Products.Include(p => p.Team).Where(p => p.IsOnSale).ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _context.Products
                .Include(p => p.Team)
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        // --- NEW: Filter by Team ID ---
        public async Task<List<Product>> GetProductsByTeamIdAsync(int teamId)
        {
            return await _context.Products
                .Include(p => p.Team)
                .Where(p => p.TeamId == teamId)
                .ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<Product>();

            return await _context.Products
                .Include(p => p.Team)
                .Where(p => p.Name.ToLower().Contains(query.ToLower()) ||
                            p.Description.ToLower().Contains(query.ToLower()) ||
                            (p.Team != null && p.Team.Name.ToLower().Contains(query.ToLower())))
                .ToListAsync();
        }
    }
}