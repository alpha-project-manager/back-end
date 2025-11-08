using Application.DataQuery;
using Application.Models;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class MeetingService : BaseService<Meeting>
{
    private readonly BaseService<TutorAttendance> _tutorAttendanceService;
    private readonly BaseService<StudentAttendance> _studentAttendanceService;
    private readonly BaseService<TodoTask> _tasksService;

    public MeetingService(IDbContextFactory<ProjectManagerDbContext> dbContextFactory,
        BaseService<TutorAttendance> tutorAttendanceService, BaseService<StudentAttendance> studentAttendanceService,
        BaseService<TodoTask> tasksService) : base(dbContextFactory)
    {
        _tutorAttendanceService = tutorAttendanceService;
        _studentAttendanceService = studentAttendanceService;
        _tasksService = tasksService;
    }

    public async Task<ServiceActionResult> DeleteMeetingsForProject(Guid projectId)
    {
        var meetings = await base.GetAsync(new DataQueryParams<Meeting>
        {
            Expression = m => m.ProjectId == projectId
        });
        var meetingsIds = meetings.Select(m => m.Id).ToArray();
        var tasks = await _tasksService.GetAsync(new DataQueryParams<TodoTask>
        {
            Expression = t => meetingsIds.Contains(t.MeetingId)
        });
        var tutorAttendances = await _tutorAttendanceService.GetAsync(new DataQueryParams<TutorAttendance>
        {
            Expression = t => meetingsIds.Contains(t.MeetingId)
        });
        var studentAttendances = await _studentAttendanceService.GetAsync(new DataQueryParams<StudentAttendance>
        {
            Expression = s => meetingsIds.Contains(s.MeetingId)
        });
        await _tasksService.RemoveRangeAsync(tasks);
        await _tutorAttendanceService.RemoveRangeAsync(tutorAttendances);
        await _studentAttendanceService.RemoveRangeAsync(studentAttendances);
        await base.RemoveRangeAsync(meetings);
        return new ServiceActionResult
        {
            Completed = true,
            Comment = ""
        };
    }
    
    public async Task<Dictionary<Meeting, TodoTask[]>> GetMeetingsForProject(Guid projectId)
    {
        var meetings = await base.GetAsync(new DataQueryParams<Meeting>
        {
            Expression = m => m.ProjectId == projectId
        });
        var result = new Dictionary<Meeting, TodoTask[]>();
        foreach (var meeting in meetings)
        {
            var tasks = await _tasksService.GetAsync(new DataQueryParams<TodoTask>
            {
                Expression = t => t.MeetingId == meeting.Id
            });
            result[meeting] = tasks;
        }
        return result;
    }
}