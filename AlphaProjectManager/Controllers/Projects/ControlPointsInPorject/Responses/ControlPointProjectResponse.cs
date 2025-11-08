using Domain.Entities;

namespace AlphaProjectManager.Controllers.Projects.ControlPointsInPorject.Responses;

public class ControlPointProjectResponse
{
    public Guid Id { get; set; }
    
    public Guid? ControlPointTemplateId { get; set; }
    
    public string? VideoUrl { get; set; }
    
    public string? Title { get; set; }
    
    public bool Completed { get; set; }
    
    public int CompanyMark { get; set; }
    
    public int UrfuMark { get; set; }
    
    public DateOnly Date { get; set; }
    
    public bool HasMarkInTeamPro { get; set; }

    public static ControlPointProjectResponse FromControlPoint(ControlPointInProject point)
    {
        return new ControlPointProjectResponse
        {
            Id = point.Id,
            ControlPointTemplateId = point.ControlPointId,
            VideoUrl = point.VideoUrl,
            Title = point.Title,
            Completed = point.Completed,
            CompanyMark = point.CompanyMark,
            UrfuMark = point.UrfuMark,
            Date = DateOnly.FromDateTime(point.Date),
            HasMarkInTeamPro = point.HasMarkInTeamPro
        };
    }
}