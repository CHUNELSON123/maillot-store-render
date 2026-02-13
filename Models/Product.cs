using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MaillotStore.Data; // Needed for Team, League, and ProductImage

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

        // --- NEW: Single Product Promo Price ---
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PromoPrice { get; set; }

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

        // =========================================================
        //  THE WATERFALL LOGIC (Effective Price Calculation)
        // =========================================================
        [NotMapped]
        public decimal EffectivePrice
        {
            get
            {
                // Priority 1: Specific Product Promo (Fixed Amount)
                if (PromoPrice.HasValue && PromoPrice.Value > 0)
                {
                    return PromoPrice.Value;
                }

                // Priority 2: Team Discount (Percentage)
                if (Team != null && Team.IsDiscountActive && Team.DiscountPercentage > 0)
                {
                    return Price - (Price * Team.DiscountPercentage / 100m);
                }

                // Priority 3: League Discount (Percentage)
                if (League != null && League.IsDiscountActive && League.DiscountPercentage > 0)
                {
                    return Price - (Price * League.DiscountPercentage / 100m);
                }

                // No Discount
                return Price;
            }
        }

        [NotMapped]
        public bool HasActiveDiscount => EffectivePrice < Price;
    }
}