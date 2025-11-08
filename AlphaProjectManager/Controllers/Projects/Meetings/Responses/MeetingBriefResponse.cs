using Domain.Entities;

namespace AlphaProjectManager.Controllers.Projects.Meetings.Responses;

public class MeetingBriefResponse
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public required DateTime DateTime { get; set; }
    
    public bool IsFinished { get; set; }
    
    public int TotalTasks { get; set; }
    
    public int CompletedTasks { get; set; }
    
    public int? ResultMark { get; set; }

    public static MeetingBriefResponse FromMeeting(Meeting meeting, TodoTask[] tasks)
    {
        var dtStr = meeting.DateTime.ToString("dd.MM.yyyy HH:mm");
        return new MeetingBriefResponse
        {
            Id = meeting.Id,
            Title = $"Встреча {dtStr}",
            DateTime = meeting.DateTime,
            IsFinished = meeting.IsFinished,
            TotalTasks = tasks.Length,
            CompletedTasks = tasks.Count(t => t.IsCompleted),
            ResultMark = meeting.ResultMark
        };
    }
}