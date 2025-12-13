using System.Collections.Generic; // Ensure this is present

namespace MaillotStore.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }

        // Main Image
        public string? ImageUrl { get; set; }

        // New Gallery Collection
        public List<ProductImage> Gallery { get; set; } = new();

        // (Legacy fields - keep them for now to avoid breaking existing data immediately)
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }

        public int Stock { get; set; }
        public string? Season { get; set; }
        public string? Sizes { get; set; }
        public bool IsFeatured { get; set; }
    }
}