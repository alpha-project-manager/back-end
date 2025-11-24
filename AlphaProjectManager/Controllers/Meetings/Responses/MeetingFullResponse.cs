using Domain.Entities;

namespace AlphaProjectManager.Controllers.Meetings.Responses;

public class MeetingFullResponse
{
    public required Guid Id { get; set; }

    public required string Description { get; set; }

    public required int ResultMark { get; set; }

    public required bool IsFinished { get; set; }

    public required DateTime DateTime { get; set; }

    public required List<TodoTaskResponse> TodoTasks { get; set; }
    
    public required List<AttendanceInfoResponse> StudentAttendances { get; set; }
    
    public required List<AttendanceInfoResponse> TutorAttendances { get; set; }

    public static MeetingFullResponse FromDomainEntities(Meeting meeting, List<TodoTask> todoTasks, 
        List<StudentAttendance> studentAttendances, List<TutorAttendance> tutorAttendances)
    {
        return new MeetingFullResponse
        {
            Id = meeting.Id,
            Description = meeting.Description,
            ResultMark = meeting.ResultMark ?? 0,
            IsFinished = meeting.IsFinished,
            DateTime = meeting.DateTime,
            TodoTasks = todoTasks.Select(TodoTaskResponse.FromTodoTask).ToList(),
            StudentAttendances = studentAttendances.Select(AttendanceInfoResponse.FromStudentAttendance).ToList(),
            TutorAttendances = tutorAttendances.Select(AttendanceInfoResponse.FromTutorAttendance).ToList()
        };
    }
}