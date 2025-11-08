namespace AlphaProjectManager.Controllers.Projects.Requests;

public class CreateNewProjectRequest
{
    public required string Title { get; set; }
    
    public Guid? CaseId { get; set; }
}