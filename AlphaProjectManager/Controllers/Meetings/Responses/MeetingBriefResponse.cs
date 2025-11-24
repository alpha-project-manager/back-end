using Domain.Entities;

namespace AlphaProjectManager.Controllers.Meetings.Responses;

public class MeetingBriefResponse
{
    public required Guid Id { get; set; }
    
    public required string Description { get; set; }
    
    public required int ResultMark { get; set; }
    
    public required bool IsFinished { get; set; }
    
    public required DateTime DateTime { get; set; }
    
    public required int TotalTasksCount { get; set; }
    
    public required int CompletedTasksCount { get; set; }
    
    public static MeetingBriefResponse FromMeeting(Meeting meeting, TodoTask[] todoTasks)
    {
        return new MeetingBriefResponse
        {
            Id = meeting.Id,
            Description = meeting.Description,
            ResultMark = meeting.ResultMark ?? 0,
            IsFinished = meeting.IsFinished,
            DateTime = meeting.DateTime,
            TotalTasksCount = todoTasks.Length,
            CompletedTasksCount = todoTasks.Count(t => t.IsCompleted)
        };
    }
}