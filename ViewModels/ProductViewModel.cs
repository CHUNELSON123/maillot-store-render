using MaillotStore.Data;
using System.ComponentModel.DataAnnotations;

namespace MaillotStore.ViewModels
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string? Category { get; set; }

        public int Stock { get; set; }
        public string? SeasonType { get; set; }
        public string? SeasonYear { get; set; }
        public bool IsFeatured { get; set; }

        public string? ImageUrl { get; set; }

        // Gallery Images (Dynamic List)
        public List<ProductImage> Gallery { get; set; } = new();
    }
}