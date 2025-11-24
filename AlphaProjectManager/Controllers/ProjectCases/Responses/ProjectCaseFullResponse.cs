using AlphaProjectManager.Controllers.Base.Responses;
using Domain.Entities;

namespace AlphaProjectManager.Controllers.ProjectCases.Responses;

public class ProjectCaseFullResponse : BaseStatusResponse
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public required string Goal { get; set; }
    
    public required string RequestedResult { get; set; }

    public required string Criteria { get; set; }
    
    public required Guid? TutorId { get; set; }

    public required string? TutorFio { get; set; }
    
    public required int MaxTeams { get; set; }
    
    public required int AcceptedTeams { get; set; }
    
    public required bool IsActive { get; set; }
    
    public required DateTime UpdatedAt { get; set; }

    public static ProjectCaseFullResponse FromProjectCase(ProjectCase projectCase)
    {
        return new ProjectCaseFullResponse
        {
            Id = projectCase.Id,
            Title = projectCase.Title,
            Description = projectCase.Description,
            Goal = projectCase.Goal,
            RequestedResult = projectCase.RequestedResult,
            Criteria = projectCase.Criteria,
            TutorId = projectCase.TutorId,
            TutorFio = projectCase.Tutor?.FullName,
            MaxTeams = projectCase.MaxTeams,
            AcceptedTeams = projectCase.AcceptedTeams,
            IsActive = projectCase.IsActive,
            Completed = true,
            Message = "",
            UpdatedAt = projectCase.UpdatedTime
        };
    }
}