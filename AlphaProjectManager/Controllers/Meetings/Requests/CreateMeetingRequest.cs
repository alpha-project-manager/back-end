namespace AlphaProjectManager.Controllers.Meetings.Requests;

public class CreateMeetingRequest
{
    public required DateTime DateTime { get; set; }
    
    public required List<string> TodoTasks { get; set; }
}