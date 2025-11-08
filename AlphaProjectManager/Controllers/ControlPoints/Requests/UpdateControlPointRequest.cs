namespace AlphaProjectManager.Controllers.ControlPoints.Requests;

public class UpdateControlPointRequest
{
    public string? Title { get; set; }
    
    public DateOnly Date { get; set; }
    
    public bool UpdateInAllProjects { get; set; }
}