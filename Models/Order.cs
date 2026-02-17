using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaillotStore.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
        public string? OptionalMessage { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Existing Status (e.g., Pending, Shipped, Delivered)
        public string Status { get; set; } = "Pending";

        // --- NEW PAYMENT FIELDS ---
        public string PaymentMethod { get; set; } = "CashOnDelivery"; // "NotchPay" or "CashOnDelivery"
        public string PaymentStatus { get; set; } = "Pending"; // "Pending", "Paid", "Failed"
        public string? PaymentReference { get; set; } // The unique ID sent to NotchPay
        // --------------------------

        public string? ReferralCode { get; set; }
        public bool IsVisibleToAdmin { get; set; } = true;
        public bool IsVisibleToInfluencer { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionRate { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}