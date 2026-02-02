using MaillotStore.Data;

namespace MaillotStore.Services.Interfaces
{
    public interface ILeagueService
    {
        Task<List<League>> GetAllLeaguesAsync();
        Task<League?> GetLeagueByIdAsync(int id);
        Task AddLeagueAsync(League league);
        Task UpdateLeagueAsync(League league);
        Task DeleteLeagueAsync(int id);
    }
}