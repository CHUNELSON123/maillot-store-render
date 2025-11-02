namespace MaillotStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }

        public string? ReferralCode { get; set; }

        public string Status { get; set; } = "Pending";
        public bool IsVisibleToAdmin { get; set; } = true;      // <-- ADD THIS LINE
        public bool IsVisibleToInfluencer { get; set; } = true; // <-- ADD THIS LINE
                                                                // Inside your Order class
        public decimal CommissionRate { get; set; }
        public decimal CommissionAmount { get; set; }

        public string? OptionalMessage { get; set; }
    }
}