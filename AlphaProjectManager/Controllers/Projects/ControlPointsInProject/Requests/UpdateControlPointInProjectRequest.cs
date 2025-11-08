using Domain.Entities;

namespace AlphaProjectManager.Controllers.Projects.ControlPointsInProject.Requests;

public class UpdateControlPointInProjectRequest
{
    public Guid? ControlPointTemplateId { get; set; }
    
    public string? VideoUrl { get; set; }
    
    public string? Title { get; set; }
    
    public bool Completed { get; set; }
    
    public int CompanyMark { get; set; }
    
    public int UrfuMark { get; set; }
    
    public DateOnly Date { get; set; }
    
    public bool HasMarkInTeamPro { get; set; }

    public void ApplyToControlPointInProject(ControlPointInProject point)
    {
        point.ControlPointId = ControlPointTemplateId;
        point.VideoUrl = VideoUrl ?? "";
        point.Title = Title ?? "";
        point.Completed = Completed;
        point.CompanyMark = CompanyMark;
        point.UrfuMark = UrfuMark;
        point.Date = Date.ToDateTime(new TimeOnly());
        point.HasMarkInTeamPro = HasMarkInTeamPro;
    }
}