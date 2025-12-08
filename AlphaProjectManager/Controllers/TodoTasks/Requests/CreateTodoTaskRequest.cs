namespace AlphaProjectManager.Controllers.TodoTasks.Requests;

public class CreateTodoTaskRequest
{
    public required string Title { get; set; }
    
    public required Guid MeetingId { get; set; }
}