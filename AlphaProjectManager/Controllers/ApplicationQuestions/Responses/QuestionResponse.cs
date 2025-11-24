using AlphaProjectManager.Controllers.Base.Responses;
using Domain.Entities.TelegramBot;

namespace AlphaProjectManager.Controllers.ApplicationQuestions.Responses;

public class QuestionResponse
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string MsgText { get; set; }
    
    public required Guid? PrevQuestionId { get; set; }
    
    public required Guid? NextQuestionId { get; set; }
    
    public static QuestionResponse FromApplicationQuestion(ApplicationQuestion question)
    {
        return new QuestionResponse
        {
            Id = question.Id,
            Title = question.Title,
            MsgText = question.MsgText,
            PrevQuestionId = question.PrevQuestionId,
            NextQuestionId = question.NextQuestionId
        };
    }
}