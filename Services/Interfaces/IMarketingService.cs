using MaillotStore.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace MaillotStore.Services.Interfaces
{
    public interface IMarketingService
    {
        Task<List<MarketingAsset>> GetAssetsBySectionAsync(string section);
        Task AddAssetAsync(MarketingAsset asset, IBrowserFile? file);
        Task DeleteAssetAsync(int id);
    }
}