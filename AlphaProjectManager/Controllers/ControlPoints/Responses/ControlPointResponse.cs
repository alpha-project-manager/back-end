using Domain.Entities;

namespace AlphaProjectManager.Controllers.ControlPoints.Responses;

public class ControlPointResponse
{
    public required Guid Id { get; set; }
    
    public string? Title { get; set; }
    
    public DateOnly Date { get; set; }

    public static ControlPointResponse FromControlPoint(ControlPoint point)
    {
        return new ControlPointResponse
        {
            Id = point.Id,
            Title = point.Title,
            Date = DateOnly.FromDateTime(point.Date)
        };
    }
}