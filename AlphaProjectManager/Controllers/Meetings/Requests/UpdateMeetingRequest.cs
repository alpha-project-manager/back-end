using Domain.Entities;

namespace AlphaProjectManager.Controllers.Meetings.Requests;

public class UpdateMeetingRequest
{
    public required string Description { get; set; }
    
    public required int ResultMark { get; set; }
    
    public required bool IsFinished { get; set; }
    
    public required DateTime DateTime { get; set; }

    public void ApplyToMeeting(Meeting meeting)
    {
        meeting.Description = Description;
        meeting.ResultMark = ResultMark;
        meeting.IsFinished = IsFinished;
        meeting.DateTime = DateTime;
    }
}