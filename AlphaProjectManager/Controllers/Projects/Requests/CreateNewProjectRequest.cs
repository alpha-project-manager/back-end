namespace AlphaProjectManager.Controllers.Projects.Requests;

public class CreateNewProjectRequest
{
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public string? MeetingUrl { get; set; }
    
    public required string TeamTitle { get; set; }
    
    public Guid? CaseId { get; set; }
}