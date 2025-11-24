using Domain.Entities;

namespace Application.Services.Meetings.Models;

public class FullMeetingInfo
{
    public required Meeting Meeting { get; set; }
    
    public required List<TodoTask> TodoTasks { get; set; }
    
    public required List<StudentAttendance> StudentAttendances { get; set; }
    
    public required List<TutorAttendance> TutorAttendances { get; set; }
}