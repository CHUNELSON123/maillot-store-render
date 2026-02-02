using MaillotStore.Models;

namespace MaillotStore.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Product>> GetFeaturedProductsAsync();
        Task<List<Product>> GetOnSaleProductsAsync();
        Task<List<Product>> GetProductsByCategoryAsync(string category);

        // --- ADD THIS NEW LINE ---
        Task<List<Product>> GetProductsByTeamIdAsync(int teamId);

        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<List<Product>> SearchProductsAsync(string query);
    }
}