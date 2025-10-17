using AlphaProjectManager.Controllers.Base.Responses;

namespace AlphaProjectManager.Controllers.ProjectCases.Responses;

public class ProjectCaseBriefResponse
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public Guid? TutorId { get; set; }

    public string? TutorFio { get; set; }
    
    public required int MaxTeams { get; set; }
    
    public required int AcceptedTeams { get; set; }
    
    public required bool IsActive { get; set; }
}