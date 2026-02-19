using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Components.Forms;

namespace MaillotStore.Services.Implementations
{
    public class CloudinaryService
    {
        private readonly Cloudinary? _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            if (!string.IsNullOrEmpty(cloudName))
            {
                var account = new Account(cloudName, apiKey, apiSecret);
                _cloudinary = new Cloudinary(account);
                _cloudinary.Api.Secure = true;
            }
        }

        public async Task<string?> UploadImageAsync(IBrowserFile file)
        {
            if (_cloudinary == null) throw new Exception("Cloudinary not configured in Render Settings.");

            // 1. Resize locally to WebP (Drastically speeds up upload and saves space)
            // 800x800 is the standard size for storefront images
            var resizedImage = await file.RequestImageFileAsync("image/webp", 800, 800);

            // 2. Read stream
            using var stream = resizedImage.OpenReadStream(20 * 1024 * 1024);

            // 3. Upload to Cloud
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.Name, stream)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary Upload Error: {uploadResult.Error.Message}");
            }

            // 4. Generate the OPTIMIZED Delivery URL
            // This forces Cloudinary to compress the image on-the-fly when users load your site
            var optimizedUrl = _cloudinary.Api.UrlImgUp
                .Secure(true)
                .Transform(new Transformation().Quality("auto").FetchFormat("auto"))
                .BuildUrl(uploadResult.PublicId);

            return optimizedUrl;
        }
    }
}