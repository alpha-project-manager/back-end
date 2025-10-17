namespace AlphaProjectManager.Controllers.ProjectCases.Requests;

public class UpdateCaseRequest
{
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public required string Goal { get; set; }
    
    public required string RequestedResult { get; set; }

    public required string Criteria { get; set; }
    
    public required Guid? TutorId { get; set; }
    
    public required int MaxTeams { get; set; }
    
    public required bool IsActive { get; set; }
}