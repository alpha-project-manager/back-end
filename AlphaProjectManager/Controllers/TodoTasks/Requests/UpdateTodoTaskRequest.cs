using Domain.Entities;

namespace AlphaProjectManager.Controllers.TodoTasks.Requests;

public class UpdateTodoTaskRequest
{
    public required string Title { get; set; }
    
    public required bool IsCompleted { get; set; }

    public void ApplyToTodoTask(TodoTask todoTask)
    {
        todoTask.Title = Title;
        todoTask.IsCompleted = IsCompleted;
    }
}