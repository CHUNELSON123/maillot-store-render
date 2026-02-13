using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaillotStore.Data
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? LogoUrl { get; set; }

        public bool DisplayOnHome { get; set; } = false;

        public int? LeagueId { get; set; }

        [ForeignKey("LeagueId")]
        public League? League { get; set; }

        // --- NEW: Team Level Discount ---
        public int DiscountPercentage { get; set; } = 0;
        public bool IsDiscountActive { get; set; } = false;
    }
}