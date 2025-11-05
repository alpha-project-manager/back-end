using Domain.Entities.TelegramBot;

namespace AlphaProjectManager.Controllers.ProjectApplications.Responses;

public class ApplicationQuestionAnswerResponse
{
    public required Guid Id { get; set; }
    
    public required long TimeStamp { get; set; }
    
    public required string QuestionTitle { get; set; }
    
    public required string Answer { get; set; }

    public static ApplicationQuestionAnswerResponse FromQuestionAnswer(ApplicationQuestionAnswer answer)
    {
        return new ApplicationQuestionAnswerResponse
        {
            TimeStamp = answer.TimeStamp,
            QuestionTitle = answer.QuestionTitle,
            Answer = answer.Answer,
            Id = answer.Id
        };
    }
}