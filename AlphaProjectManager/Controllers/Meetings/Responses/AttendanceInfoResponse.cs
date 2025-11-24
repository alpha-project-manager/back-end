using Domain.Entities;

namespace AlphaProjectManager.Controllers.Meetings.Responses;

public class AttendanceInfoResponse
{
    public required Guid PersonId { get; set; }
    
    public required string FullName { get; set; }
    
    public required bool Attended { get; set; }

    public static AttendanceInfoResponse FromStudentAttendance(StudentAttendance attendance)
    {
        return new AttendanceInfoResponse
        {
            PersonId = attendance.StudentId,
            FullName = attendance.Student?.FullName ?? "",
            Attended = attendance.Attended
        };
    }
    
    public static AttendanceInfoResponse FromTutorAttendance(TutorAttendance attendance)
    {
        return new AttendanceInfoResponse
        {
            PersonId = attendance.TutorId,
            FullName = attendance.Tutor?.FullName ?? "",
            Attended = attendance.Attended
        };
    }
}