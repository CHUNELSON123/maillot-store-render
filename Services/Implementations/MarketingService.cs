using MaillotStore.Data;
using MaillotStore.Models;
using MaillotStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;

namespace MaillotStore.Services.Implementations
{
    public class MarketingService : IMarketingService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IWebHostEnvironment _environment;

        public MarketingService(IDbContextFactory<ApplicationDbContext> factory, IWebHostEnvironment environment)
        {
            _factory = factory;
            _environment = environment;
        }

        public async Task<List<MarketingAsset>> GetAssetsBySectionAsync(string section)
        {
            using var context = _factory.CreateDbContext();
            return await context.MarketingAssets
                .Where(a => a.Section == section)
                .OrderBy(a => a.DisplayOrder)
                .ThenByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task AddAssetAsync(MarketingAsset asset, IBrowserFile? file)
        {
            using var context = _factory.CreateDbContext();

            if (file != null)
            {
                var folderName = asset.Section == "HeroBanner" ? "banners" : "showcase";
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folderName);

                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Allow up to 100MB for video uploads
                long maxFileSize = 1024 * 1024 * 100;

                await using FileStream fs = new(filePath, FileMode.Create);
                await file.OpenReadStream(maxFileSize).CopyToAsync(fs);

                asset.Url = $"/uploads/{folderName}/{fileName}";
            }

            context.MarketingAssets.Add(asset);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAssetAsync(int id)
        {
            using var context = _factory.CreateDbContext();
            var asset = await context.MarketingAssets.FindAsync(id);
            if (asset != null)
            {
                if (asset.Url.StartsWith("/uploads/"))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, asset.Url.TrimStart('/'));
                    if (File.Exists(filePath)) File.Delete(filePath);
                }

                context.MarketingAssets.Remove(asset);
                await context.SaveChangesAsync();
            }
        }
    }
}