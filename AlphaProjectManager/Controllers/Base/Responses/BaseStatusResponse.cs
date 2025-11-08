using Application.Models;

namespace AlphaProjectManager.Controllers.Base.Responses;

public class BaseStatusResponse
{
    public required bool Completed { get; set; }
    
    public required string Message { get; set; }

    public static BaseStatusResponse FromServiceActionResult(ServiceActionResult actionResult)
    {
        return new BaseStatusResponse
        {
            Completed = actionResult.Completed,
            Message = actionResult.Comment
        };
    }
}