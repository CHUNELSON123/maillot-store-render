using System.ComponentModel.DataAnnotations;

namespace MaillotStore.Models
{
    public class MarketingAsset
    {
        [Key]
        public int Id { get; set; }

        // "HeroBanner" or "ShopShowcase"
        [Required]
        public string Section { get; set; } = "HeroBanner";

        // "Image" or "Video"
        public string MediaType { get; set; } = "Image";

        // The link to the image/video (can be a local /uploads/... path or an external https://...)
        [Required]
        public string Url { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }
}