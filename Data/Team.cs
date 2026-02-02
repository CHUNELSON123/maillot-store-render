using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Needed for ForeignKey attribute

namespace MaillotStore.Data
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string LogoUrl { get; set; } = string.Empty;

        public bool DisplayOnHome { get; set; } = true;

        // --- NEW: Link to League ---
        // Every team MUST belong to a league now.
        public int LeagueId { get; set; }

        [ForeignKey("LeagueId")]
        public League? League { get; set; }
    }
}