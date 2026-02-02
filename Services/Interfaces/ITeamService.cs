using MaillotStore.Data;

namespace MaillotStore.Services.Interfaces
{
    public interface ITeamService
    {
        Task<List<Team>> GetAllTeamsAsync();
        Task<List<Team>> GetHomeDisplayTeamsAsync();
        Task<Team?> GetTeamByIdAsync(int id);
        Task AddTeamAsync(Team team);
        Task UpdateTeamAsync(Team team);
        Task DeleteTeamAsync(int id);
    }
}