using Application.DataQuery;
using Application.Models;
using Application.Services.Meetings.Models;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Meetings;

public class MeetingService : BaseService<Meeting>
{
    private readonly BaseService<TutorAttendance> _tutorAttendanceService;
    private readonly BaseService<StudentAttendance> _studentAttendanceService;
    private readonly BaseService<TodoTask> _tasksService;
    private readonly ProjectsService _projectService;
    private readonly BaseService<StudentInProject> _studentsInProjectService;

    public MeetingService(IDbContextFactory<ProjectManagerDbContext> dbContextFactory,
        BaseService<TutorAttendance> tutorAttendanceService, BaseService<StudentAttendance> studentAttendanceService,
        BaseService<TodoTask> tasksService, ProjectsService projectService, BaseService<StudentInProject> studentsInProjectService) : base(dbContextFactory)
    {
        _tutorAttendanceService = tutorAttendanceService;
        _studentAttendanceService = studentAttendanceService;
        _tasksService = tasksService;
        _projectService = projectService;
        _studentsInProjectService = studentsInProjectService;
    }

    public async Task<FullMeetingInfo?> CreateNewMeeting(Guid projectId, DateTime dateTime, List<string> todoTasks)
    {
        var project = await _projectService.GetByIdOrDefaultAsync(projectId);
        if (project == null)
        {
            return null;
        }
        
        var meeting = new Meeting
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Description = "",
            ResultMark = null,
            IsFinished = false,
            DateTime = dateTime.ToUniversalTime()
        };
        await base.CreateAsync(meeting);
        
        var result = new FullMeetingInfo
        {
            Meeting = meeting,
            TodoTasks = [],
            StudentAttendances = [],
            TutorAttendances = []
        };

        foreach (var taskTitle in todoTasks)
        {
            var task = new TodoTask
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                IsCompleted = false,
                Title = taskTitle
            };
            await _tasksService.CreateAsync(task);
            result.TodoTasks.Add(task);
        }
        
        var students = await _studentsInProjectService.GetAsync(new DataQueryParams<StudentInProject>
        {
            Expression = s => s.ProjectId == projectId
        });
        foreach (var studentInProject in students)
        {
            var studentAttendance = new StudentAttendance
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                StudentId = studentInProject.StudentId,
                Attended = false
            };
            await _studentAttendanceService.CreateAsync(studentAttendance);
            result.StudentAttendances.Add(studentAttendance);
        }
        
        if (project.TutorId.HasValue)
        {
            var tutorAttendance = new TutorAttendance
            {
                Id = Guid.NewGuid(),
                MeetingId = meeting.Id,
                TutorId = project.TutorId.Value,
                Attended = false
            };
            await _tutorAttendanceService.CreateAsync(tutorAttendance);
            result.TutorAttendances.Add(tutorAttendance);
        }

        return result;
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
    
    public async Task<ServiceActionResult> DeleteMeeting(Guid meetingId)
    {
        var meeting = await base.GetByIdOrDefaultAsync(meetingId);
        if (meeting == null)
        {
            return new ServiceActionResult
            {
                Completed = false,
                Comment = $"Meeting with id {meetingId} not found."
            };
        }
        var tasks = await _tasksService.GetAsync(new DataQueryParams<TodoTask>
        {
            Expression = t => t.MeetingId == meeting.Id
        });
        var tutorAttendances = await _tutorAttendanceService.GetAsync(new DataQueryParams<TutorAttendance>
        {
            Expression = t => t.MeetingId == meeting.Id
        });
        var studentAttendances = await _studentAttendanceService.GetAsync(new DataQueryParams<StudentAttendance>
        {
            Expression = s => s.MeetingId == meeting.Id
        });
        await _tasksService.RemoveRangeAsync(tasks);
        await _tutorAttendanceService.RemoveRangeAsync(tutorAttendances);
        await _studentAttendanceService.RemoveRangeAsync(studentAttendances);
        await base.TryRemoveAsync(meeting.Id);
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
            Expression = m => m.ProjectId == projectId,
            Sorting = new SortingParams<Meeting>
            {
                OrderBy = m => m.DateTime,
                Ascending = false
            }
        });
        var result = new Dictionary<Meeting, TodoTask[]>();
        foreach (var meeting in meetings)
        {
            var tasks = await _tasksService.GetAsync(new DataQueryParams<TodoTask>
            {
                Expression = t => t.MeetingId == meeting.Id,
                Sorting = new SortingParams<TodoTask>
                {
                    OrderBy = t => t.IsCompleted
                }
            });
            result[meeting] = tasks;
        }
        return result;
    }

    public async Task<FullMeetingInfo?> GetFullMeetingInfoById(Guid meetingId)
    {
        var meeting = await base.GetByIdOrDefaultAsync(meetingId);
        if (meeting == null)
        {
            return null;
        }
        var tutorAttendances = await _tutorAttendanceService.GetAsync(new DataQueryParams<TutorAttendance>
        {
            Expression = t => t.MeetingId == meetingId
        });
        var studentAttendances = await _studentAttendanceService.GetAsync(new DataQueryParams<StudentAttendance>
        {
            Expression = t => t.MeetingId == meetingId
        });
        var todoTasks = await _tasksService.GetAsync(new DataQueryParams<TodoTask>
        {
            Expression = t => t.MeetingId == meetingId,
            Sorting = new SortingParams<TodoTask>
            {
                OrderBy = t => t.IsCompleted
            }
        });
        return new FullMeetingInfo
        {
            Meeting = meeting,
            TodoTasks = todoTasks.ToList(),
            StudentAttendances = studentAttendances.ToList(),
            TutorAttendances = tutorAttendances.ToList()
        };
    }
    
    public async Task<TutorAttendance> SetAttendanceOfTutor(Guid meetingId, Guid tutorId, bool attended)
    {
        var tutorAttendances = await _tutorAttendanceService.GetAsync(new DataQueryParams<TutorAttendance>
        {
            Expression = at => at.MeetingId == meetingId,
            Filters = [at => at.TutorId == tutorId]
        });
        if (tutorAttendances.Length == 0)
        {
            var attendance = new TutorAttendance
            {
                Id = Guid.NewGuid(),
                MeetingId = meetingId,
                TutorId = tutorId,
                Attended = attended
            };
            await _tutorAttendanceService.CreateAsync(attendance);
            return attendance;
        }
        var existingAttendance = tutorAttendances[0];
        existingAttendance.Attended = attended;
        await _tutorAttendanceService.UpdateAsync(existingAttendance);
        return existingAttendance;
    }
    
    public async Task<StudentAttendance> SetAttendanceOfStudent(Guid meetingId, Guid studentId, bool attended)
    {
        var foundAttendances = await _studentAttendanceService.GetAsync(new DataQueryParams<StudentAttendance>
        {
            Expression = at => at.MeetingId == meetingId,
            Filters = [at => at.StudentId == studentId]
        });
        if (foundAttendances.Length == 0)
        {
            var attendance = new StudentAttendance
            {
                Id = Guid.NewGuid(),
                MeetingId = meetingId,
                StudentId = studentId,
                Attended = attended
            };
            await _studentAttendanceService.CreateAsync(attendance);
            return attendance;
        }
        var existingAttendance = foundAttendances[0];
        existingAttendance.Attended = attended;
        await _studentAttendanceService.UpdateAsync(existingAttendance);
        return existingAttendance;
    }
}