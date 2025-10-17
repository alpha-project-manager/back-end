using AlphaProjectManager.Controllers.Base.Responses;

namespace AlphaProjectManager.Controllers.ProjectCases.Responses;

public class ProjectCaseListResponse : BaseStatusResponse
{
    public required ProjectCaseBriefResponse[] Cases { get; set; }
}