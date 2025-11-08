namespace AlphaProjectManager.Controllers.Projects.Meetings.Responses;

public class MeetingAttendanceResponse
{
    public Guid TutorId { get; set; }

    public string MemberFullname { get; set; } = "";
    
    public bool Attended { get; set; }
}