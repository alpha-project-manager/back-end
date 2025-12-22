namespace AlphaProjectManager.Controllers.Meetings.Requests;

public class CreateMeetingRequest
{
    public required DateTime DateTime { get; set; }
    
    public string? Description { get; set; }
    
    public int ResultMark { get; set; }
    
    public bool IsFinished { get; set; }
    
    public required List<TodoTaskRequest> TodoTasks { get; set; }
}