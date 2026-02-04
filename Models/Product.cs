using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MaillotStore.Data; // Needed for Team and ProductImage

namespace MaillotStore.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public string Size { get; set; }

        public bool IsOnSale { get; set; }

        // --- NEW PROPERTIES ADDED TO FIX ERRORS ---
        public int Stock { get; set; }

        public bool IsFeatured { get; set; }

        public string? Season { get; set; }

        // Navigation property for Gallery Images
        public List<ProductImage> Gallery { get; set; } = new();
        public string? Version { get; set; }
        // --- EXISTING RELATIONSHIPS ---
        public int? TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team? Team { get; set; }
        public int? LeagueId { get; set; }

        [ForeignKey("LeagueId")]
        public League? League { get; set; }
    }
}