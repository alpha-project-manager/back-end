using AlphaProjectManager.Controllers.Base.Responses;

namespace AlphaProjectManager.Controllers.ApplicationQuestions.Responses;

public class QuestionResponse
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string MsgText { get; set; }
    
    public required Guid? PrevQuestionId { get; set; }
    
    public required Guid? NextQuestionId { get; set; }
}