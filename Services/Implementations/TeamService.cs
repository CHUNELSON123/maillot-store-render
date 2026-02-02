using MaillotStore.Data;
using MaillotStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaillotStore.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public TeamService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Team>> GetAllTeamsAsync()
        {
            using var context = _factory.CreateDbContext();
            // Include League for the admin table display
            return await context.Teams.Include(t => t.League).ToListAsync();
        }

        public async Task<List<Team>> GetHomeDisplayTeamsAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.Teams.Where(t => t.DisplayOnHome).ToListAsync();
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            using var context = _factory.CreateDbContext();
            return await context.Teams.Include(t => t.League).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddTeamAsync(Team team)
        {
            using var context = _factory.CreateDbContext();
            context.Teams.Add(team);
            await context.SaveChangesAsync();
        }

        public async Task UpdateTeamAsync(Team team)
        {
            using var context = _factory.CreateDbContext();
            context.Teams.Update(team);
            await context.SaveChangesAsync();
        }

        public async Task DeleteTeamAsync(int id)
        {
            using var context = _factory.CreateDbContext();
            var team = await context.Teams.FindAsync(id);
            if (team != null)
            {
                context.Teams.Remove(team);
                await context.SaveChangesAsync();
            }
        }
    }
}