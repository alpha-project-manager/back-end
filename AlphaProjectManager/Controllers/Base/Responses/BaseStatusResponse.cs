namespace AlphaProjectManager.Controllers.Base.Responses;

public class BaseStatusResponse
{
    public required bool Completed { get; set; }
    
    public required string Message { get; set; }
}