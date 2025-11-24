using Domain.Entities;

namespace AlphaProjectManager.Controllers.Meetings.Responses;

public class TodoTaskResponse
{
    public required Guid Id { get; set; }
    
    public required bool IsCompleted { get; set; }
    
    public required string Title { get; set; }

    public static TodoTaskResponse FromTodoTask(TodoTask task)
    {
        return new TodoTaskResponse
        {
            Id = task.Id,
            IsCompleted = task.IsCompleted,
            Title = task.Title
        };
    }
}