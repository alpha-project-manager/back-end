namespace AlphaProjectManager.Controllers.ApplicationQuestions.Requests;

public class UpdateQuestionRequest
{
    public required string Title { get; set; }
    
    public required string MsgText { get; set; }
}