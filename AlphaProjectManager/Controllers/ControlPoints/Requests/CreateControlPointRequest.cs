namespace AlphaProjectManager.Controllers.ControlPoints.Requests;

public class CreateControlPointRequest
{
    public required string Title { get; set; }
    
    public DateOnly Date { get; set; }
    
    public bool CreateInAllProjects { get; set; }
}