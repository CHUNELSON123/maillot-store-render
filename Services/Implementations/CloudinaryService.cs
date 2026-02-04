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

            // 1. Resize locally to speed up upload (Keep your existing resizing logic!)
            var resizedImage = await file.RequestImageFileAsync("image/jpeg", 1000, 1000);

            // 2. Read stream
            using var stream = resizedImage.OpenReadStream(20 * 1024 * 1024);

            // 3. Upload to Cloud
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.Name, stream),
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // 4. Return the Cloud URL (This never gets deleted)
            return uploadResult.SecureUrl.ToString();
        }
    }
}