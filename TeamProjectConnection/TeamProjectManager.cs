using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using TeamProjectConnection.Intefaces;
using TeamProjectConnection.Models;
using TeamProjectConnection.Models.TeamProResponses.Catalog;
using TeamProjectConnection.Models.TeamProResponses.Periods;
using TeamProjectConnection.Models.TeamProResponses.ProjectDetails;
using TeamProjectConnection.Models.TeamProResponses.Team;
using TeamProjectConnection.Models.TeamProResponses.User;

namespace TeamProjectConnection;

public class TeamProjectManager : ITeamProjectManager
{
    private const string ApiBaseUrl = "https://teamproject.urfu.ru/api/v2";
    private const string PhotoBaseUrl = "https://teamproject.urfu.ru";
    
    private readonly ITeamProAuthManager _teamProAuthManager;
    
    private readonly JsonSerializerSettings _snakeCaseSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
        MissingMemberHandling = MissingMemberHandling.Ignore
    };
    
    private readonly JsonSerializerSettings _camelCaseSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
        MissingMemberHandling = MissingMemberHandling.Ignore
    };
    
    public TeamProjectManager(ITeamProAuthManager teamProAuthManager)
    {
        _teamProAuthManager = teamProAuthManager;
    }
    
    public async Task<TeamProUserResponse?> GetUserDataAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ApiBaseUrl + "/user");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var result = await SendRequestAsync<TeamProUserResponse>(request, true, _snakeCaseSerializerSettings);
        if (!string.IsNullOrWhiteSpace(result?.Person?.Photo))
        {
            result.Person.Photo = PhotoBaseUrl + result.Person.Photo;
        }
        return result;
    }
    
    public async Task<TeamProPeriodResponse?> GetCurrentPeriod()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ApiBaseUrl + "/filters/periods");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var allPeriods = await SendRequestAsync<TeamProAllPeriodsResponse>(request, true, _snakeCaseSerializerSettings);
        return allPeriods?.Current;
    }
    
    public async Task<TeamProCatalogResponse?> GetProjectsForPeriod(int year, int semester, int size = 100, int page = 1)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ApiBaseUrl + $"/catalog?status=active&year={year}&semester={semester}&size={size}&page={page}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var catalog = await SendRequestAsync<TeamProCatalogResponse>(request, true, _camelCaseSerializerSettings);
        foreach (var project in catalog?.Items ?? [])
        {
            if (!string.IsNullOrWhiteSpace(project.MainCurator?.AvatarUrl))
            {
                project.MainCurator.AvatarUrl = PhotoBaseUrl + project.MainCurator?.AvatarUrl;
            }

            foreach (var student in project.Students.Where(student => !string.IsNullOrWhiteSpace(student.AvatarUrl)))
            {
                student.AvatarUrl = PhotoBaseUrl + student.AvatarUrl;
            }
        }
        return catalog;
    }

    public async Task<TeamProDetailsResponse?> GetDetailsForProject(Guid projectId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ApiBaseUrl + $"/workspaces/{projectId}/details");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var detailsResponse = await SendRequestAsync<TeamProDetailsResponse>(request, true, _camelCaseSerializerSettings);
        return detailsResponse;
    }
    
    public async Task<TeamProTeamResponse?> GetTeamForProject(Guid projectId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ApiBaseUrl + $"/workspaces/{projectId}/widgets/team");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var teamResponse = await SendRequestAsync<TeamProTeamResponse>(request, true, _camelCaseSerializerSettings);
        if (teamResponse != null)
        {
            if (!string.IsNullOrWhiteSpace(teamResponse.MainCurator?.Photo))
            {
                teamResponse.MainCurator.Photo = PhotoBaseUrl + teamResponse.MainCurator.Photo;
            }
            foreach (var member in teamResponse.Students.Concat(teamResponse.AdditionalCurators))
            {
                if (!string.IsNullOrWhiteSpace(member.Photo))
                {
                    member.Photo = PhotoBaseUrl + member.Photo;
                }
            }
        }
        
        return teamResponse;
    }
    
    private async Task<TResponse?> SendRequestAsync<TResponse>(HttpRequestMessage request, bool addBearer, 
        JsonSerializerSettings settings) where TResponse : class
    {
        if (addBearer)
        {
            var token = CheckAuthorization();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }
        
        var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<TResponse>(content, settings);
        if (result == null)
        {
            Log.Error("Failed to deserialize response from TeamProject. Uri: {uri}. Model: {modelName}. Response: {json}", 
                request.RequestUri, nameof(TResponse), content);
            return null;
        }
        return result;
    }
    
    private TokenResponse CheckAuthorization()
    {
        if (!_teamProAuthManager.TryGetSavedToken(out var token) || token == null)
        {
            throw new InvalidOperationException("TeamProAuthManager is not authorized for this scope.");
        }

        return token;
    }
}