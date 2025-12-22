namespace AlphaProjectManager.Controllers.Meetings.Requests;

public class TodoTaskRequest
{
    public required string Title { get; set; }
    
    public bool Completed { get; set; }
}