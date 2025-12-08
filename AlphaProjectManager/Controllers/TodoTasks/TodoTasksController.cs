using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.Meetings.Responses;
using AlphaProjectManager.Controllers.Shared;
using AlphaProjectManager.Controllers.TodoTasks.Requests;
using Application.Services;
using Application.Services.Meetings;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.TodoTasks;

[Route("/api/tasks")]
public class TodoTasksController : ControllerBase
{
    private readonly MeetingService _meetingService;
    private readonly BaseService<TodoTask> _tasksService;

    public TodoTasksController(MeetingService meetingService, BaseService<TodoTask> tasksService)
    {
        _meetingService = meetingService;
        _tasksService = tasksService;
    }
    
    /// <summary>
    /// Создать новую задачу
    /// </summary>
    [HttpPost("")]
    [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateNewTask([FromBody] CreateTodoTaskRequest dto)
    {
        var task = new TodoTask
        {
            Id = Guid.NewGuid(),
            MeetingId = dto.MeetingId,
            Title = dto.Title
        };
        await _tasksService.CreateAsync(task);
        return Ok(TodoTaskResponse.FromTodoTask(task));
    }
    
    /// <summary>
    /// Удалить задачу
    /// </summary>
    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid taskId)
    {
        var result = await _tasksService.TryRemoveAsync(taskId);
        if (result)
        {
            return SharedResponses.SuccessRequest("Task deleted.");
        }
        return SharedResponses.NotFoundObjectResponse<TodoTask>(taskId);
    }
    
    /// <summary>
    /// Обновить информацию о задаче
    /// </summary>
    [HttpPut("{taskId:guid}")]
    [ProducesResponseType(typeof(TodoTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTodoTask([FromRoute] Guid taskId, [FromBody] UpdateTodoTaskRequest dto)
    {
        var task = await _tasksService.GetByIdOrDefaultAsync(taskId);
        if (task == null)
        {
            return SharedResponses.NotFoundObjectResponse<TodoTask>(taskId);
        }
        dto.ApplyToTodoTask(task);
        await _tasksService.UpdateAsync(task);
        return Ok(TodoTaskResponse.FromTodoTask(task));
    }
    
    /// <summary>
    /// Отметить задачу как выполненную / невыполненную
    /// </summary>
    [HttpPut("{taskId:guid}/complete")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTodoTask([FromRoute] Guid taskId, [FromQuery] bool? completed)
    {
        var task = await _tasksService.GetByIdOrDefaultAsync(taskId);
        if (task == null)
        {
            return SharedResponses.NotFoundObjectResponse<TodoTask>(taskId);
        }
        task.IsCompleted = completed ?? true;
        await _tasksService.UpdateAsync(task);
        return Ok(TodoTaskResponse.FromTodoTask(task));
    }
}