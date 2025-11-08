using AlphaProjectManager.Controllers.Base.Responses;

namespace AlphaProjectManager.Controllers.Projects.Responses;

public class ImportProjectsResponse : BaseStatusResponse
{
    public required List<ProjectBriefResponse> CreatedProjects { get; set; }
    
    public required List<ProjectBriefResponse> UpdatedProjects { get; set; }
}