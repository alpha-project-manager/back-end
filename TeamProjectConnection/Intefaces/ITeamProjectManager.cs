using TeamProjectConnection.Models.TeamProResponses.Catalog;
using TeamProjectConnection.Models.TeamProResponses.Periods;
using TeamProjectConnection.Models.TeamProResponses.ProjectDetails;
using TeamProjectConnection.Models.TeamProResponses.Team;
using TeamProjectConnection.Models.TeamProResponses.User;

namespace TeamProjectConnection.Intefaces;

public interface ITeamProjectManager
{
    public Task<TeamProUserResponse?> GetUserDataAsync();

    public Task<TeamProPeriodResponse?> GetCurrentPeriod();

    public Task<TeamProCatalogResponse?> GetProjectsForPeriod(int year, int semester, int size = 100, int page = 1);

    public Task<TeamProDetailsResponse?> GetDetailsForProject(Guid projectId);

    public Task<TeamProTeamResponse?> GetTeamForProject(Guid projectId);
    
    public ITeamProAuthManager AuthManager { get; }
}