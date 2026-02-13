using System.ComponentModel.DataAnnotations;

namespace MaillotStore.Data
{
    public class League
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Team> Teams { get; set; } = new();

        // --- NEW: League Level Discount ---
        public int DiscountPercentage { get; set; } = 0;
        public bool IsDiscountActive { get; set; } = false;
    }
}