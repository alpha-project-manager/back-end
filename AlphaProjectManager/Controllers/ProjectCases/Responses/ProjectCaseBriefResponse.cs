using AlphaProjectManager.Controllers.Base.Responses;
using Domain.Entities;
using Domain.Enums;

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
    
    public required DateTime UpdatedAt { get; set; }
    
    public required Dictionary<CaseReactionType, List<CaseVoteResponse>> Votes { get; set; }

    public static ProjectCaseBriefResponse FromProjectCase(ProjectCase projectCase, CaseVote[] votes)
    {
        return new ProjectCaseBriefResponse
        {
            Id = projectCase.Id,
            Title = projectCase.Title,
            TutorId = projectCase.TutorId,
            TutorFio = projectCase.Tutor?.FullName,
            MaxTeams = projectCase.MaxTeams,
            AcceptedTeams = projectCase.AcceptedTeams,
            IsActive = projectCase.IsActive,
            UpdatedAt = projectCase.UpdatedTime,
            Votes = votes.GroupBy(v => v.ReactionType)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(v => new CaseVoteResponse
                    {
                        UserId = v.UserId,
                        FullName = v.User.FullName
                    }).ToList())
        };
    }
}