using AlphaProjectManager.Controllers.Base.Responses;
using Domain.Entities;
using Domain.Entities.TelegramBot;
using Domain.Enums;

namespace AlphaProjectManager.Controllers.ProjectApplications.Responses;

public class ApplicationResponse : BaseStatusResponse
{
    public required Guid Id { get; set; }
    
    public required Guid CaseId { get; set; }
    
    public required string CaseTitle { get; set; }
    
    public required string TeamTitle { get; set; }
    
    public required string TelegramUsername { get; set; }
    
    public required ApplicationStatus Status { get; set; }
    
    public required ApplicationQuestionAnswerResponse[] QuestionResponses { get; set; }
    
    public required MessageResponse[] Messages { get; set; }

    public static ApplicationResponse FromDomainEntities(ProjectApplication application, ApplicationQuestionAnswer[] answers,
        ApplicationMessage[] messages)
    {
        return new ApplicationResponse
        {
            Id = application.Id,
            CaseId = application.CaseId,
            CaseTitle = application.ProjectCase?.Title ?? "",
            TeamTitle = application.TeamTitle,
            TelegramUsername = application.TelegramUsername,
            Status = application.Status,
            QuestionResponses = answers.Select(ApplicationQuestionAnswerResponse.FromQuestionAnswer)
                .ToArray(),
            Messages = messages.Select(MessageResponse.FromApplicationMessage)
                .ToArray(),
            Completed = true,
            Message = ""
        };
    }
}