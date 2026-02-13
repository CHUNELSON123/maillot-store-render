using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaillotStore.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // --- The "Cookie" Link ---
        [Required]
        public string GuestId { get; set; } = string.Empty; // This stores the cookie value (e.g. "guest_xyz123")

        public string? UserId { get; set; } // Optional: If they log in later, we fill this.

        // --- Product Details ---
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = default!;

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please select a size")]
        public string Size { get; set; } = string.Empty;

        // --- Customization ---
        public string? CustomName { get; set; }
        public int? CustomNumber { get; set; }

        // --- Admin / Cleanup ---
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}