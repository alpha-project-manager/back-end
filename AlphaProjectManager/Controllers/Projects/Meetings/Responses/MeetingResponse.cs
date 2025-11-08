namespace AlphaProjectManager.Controllers.Projects.Meetings.Responses;

public class MeetingResponse
{
    public Guid Id { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public required string Title { get; set; }
    
    public string? Description { get; set; }
    
    public int ResultMark { get; set; }
    
    public bool IsFinished { get; set; }
    
    public DateTime DateTime { get; set; }

    public required List<MeetingAttendanceResponse> StudentsAttendances { get; set; }

    public required List<MeetingAttendanceResponse> TutorsAttendances { get; set; }
    
    public required List<TodoTaskResponse> Tasks { get; set; }
}