using System.ComponentModel.DataAnnotations;

namespace MaillotStore.Data
{
    public class League
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // e.g. "Premier League"

        // This allows us to easily get all teams belonging to this league
        public List<Team> Teams { get; set; } = new();
    }
}