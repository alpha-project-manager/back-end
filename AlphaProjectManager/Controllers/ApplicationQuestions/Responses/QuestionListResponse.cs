using AlphaProjectManager.Controllers.Base.Responses;

namespace AlphaProjectManager.Controllers.ApplicationQuestions.Responses;

public class QuestionListResponse : BaseStatusResponse
{
    public required QuestionResponse[] Questions { get; set; }
}