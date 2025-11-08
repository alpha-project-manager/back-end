namespace AlphaProjectManager.Controllers.Projects.Meetings.Responses;

public class TodoTaskResponse
{
    public Guid Id { get; set; }
    
    public bool IsCompleted { get; set; }

    public string Title { get; set; } = "";
}