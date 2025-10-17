using AlphaProjectManager.Controllers.Base.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.Shared;

public static class SharedResponses
{
    public static BadRequestObjectResult FailedRequest(string msg)
    {
        return new BadRequestObjectResult(new BaseStatusResponse
        {
            Message = msg,
            Completed = false
        });
    }
    
    public static NotFoundObjectResult NotFoundObjectResponse<T>(Guid providedId)
    {
        return new NotFoundObjectResult(new BaseStatusResponse
        {
            Message = $"Object of type {typeof(T).Name} with provided ID \"{providedId}\" wasn't found.",
            Completed = false
        });
    }
}