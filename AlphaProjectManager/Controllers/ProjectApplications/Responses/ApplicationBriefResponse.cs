using Domain.Entities;
using Domain.Enums;

namespace AlphaProjectManager.Controllers.ProjectApplications.Responses;

public class ApplicationBriefResponse
{
    public required Guid Id { get; set; }
    
    public required Guid CaseId { get; set; }
    
    public required string CaseTitle { get; set; }
    
    public required string TeamTitle { get; set; }
    
    public required ApplicationStatus Status { get; set; }
    
    public required DateTime UpdatedAt { get; set; }
    
    public required int UnreadMessagesCount { get; set; }

    public static ApplicationBriefResponse FromApplication(ProjectApplication application, ApplicationMessage[] messages)
    {
        return new ApplicationBriefResponse
        {
            Id = application.Id,
            CaseId = application.CaseId,
            CaseTitle = application.ProjectCase?.Title ?? "",
            TeamTitle = application.TeamTitle,
            Status = application.Status,
            UpdatedAt = application.UpdatedTime,
            UnreadMessagesCount = messages.Count(m => !m.IsRead)
        };
    }
}