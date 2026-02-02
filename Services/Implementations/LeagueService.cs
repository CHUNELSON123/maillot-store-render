using MaillotStore.Data;
using MaillotStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaillotStore.Services.Implementations
{
    public class LeagueService : ILeagueService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public LeagueService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<League>> GetAllLeaguesAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.Leagues
                                 .Include(l => l.Teams)
                                 .ToListAsync();
        }

        public async Task<League?> GetLeagueByIdAsync(int id)
        {
            using var context = _factory.CreateDbContext();
            return await context.Leagues.FindAsync(id);
        }

        public async Task AddLeagueAsync(League league)
        {
            using var context = _factory.CreateDbContext();
            context.Leagues.Add(league);
            await context.SaveChangesAsync();
        }

        public async Task UpdateLeagueAsync(League league)
        {
            using var context = _factory.CreateDbContext();
            context.Leagues.Update(league);
            await context.SaveChangesAsync();
        }

        public async Task DeleteLeagueAsync(int id)
        {
            using var context = _factory.CreateDbContext();
            var league = await context.Leagues.FindAsync(id);
            if (league != null)
            {
                context.Leagues.Remove(league);
                await context.SaveChangesAsync();
            }
        }
    }
}