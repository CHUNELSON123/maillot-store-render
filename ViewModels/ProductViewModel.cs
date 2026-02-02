using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using MaillotStore.Data;

namespace MaillotStore.ViewModels
{
    public class ProductViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        // Made optional
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        // Made optional (Admin doesn't need to fill this)
        public string? Size { get; set; }

        public int Stock { get; set; } = 1;

        public bool IsOnSale { get; set; }

        public bool IsFeatured { get; set; }

        public string? SeasonType { get; set; }
        public string? SeasonYear { get; set; }

        public string? ImageUrl { get; set; }
        public IBrowserFile? Image { get; set; }

        public List<ProductImage> Gallery { get; set; } = new();
        public string? Version { get; set; }
        public int? TeamId { get; set; }
    }
}