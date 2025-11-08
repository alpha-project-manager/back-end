using Application.DataQuery;
using Application.Models;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ProjectsService : BaseService<Project>
{
    private readonly BaseService<ControlPointInProject> _controlPointService;
    private readonly BaseService<StudentInProject> _studentInProjectService;
    private readonly MeetingService _meetingService;

    public ProjectsService(IDbContextFactory<ProjectManagerDbContext> dbContextFactory, 
        BaseService<ControlPointInProject> controlPointService, BaseService<StudentInProject> studentInProjectService,
        MeetingService meetingService) : base(dbContextFactory)
    {
        _controlPointService = controlPointService;
        _studentInProjectService = studentInProjectService;
        _meetingService = meetingService;
    }

    public async Task<Project[]> GetProjectWithStudent(Guid studentId)
    {
        var studentsInProjects = await _studentInProjectService.GetAsync(new DataQueryParams<StudentInProject>
        {
            Expression = s => s.StudentId == studentId
        });
        var projectIds = studentsInProjects.Select(s => s.ProjectId).ToArray();
        return await base.GetAsync(new DataQueryParams<Project>
        {
            Expression = p => projectIds.Contains(p.Id)
        });
    }
    
    public async Task<ServiceActionResult> DeleteProject(Guid projectId)
    {
        var project = await base.GetByIdOrDefaultAsync(projectId);
        if (project == null)
        {
            return ServiceActionResult.Failed($"Project with provided ID \"{projectId}\" wasn't found.");
        }

        var controlPoints = await _controlPointService.GetAsync(new DataQueryParams<ControlPointInProject>
        {
            Expression = p => p.ProjectId == projectId
        });
        await _controlPointService.RemoveRangeAsync(controlPoints);
        
        var studentsInProject = await _studentInProjectService.GetAsync(new DataQueryParams<StudentInProject>
        {
            Expression = s => s.ProjectId == projectId
        });
        await _studentInProjectService.RemoveRangeAsync(studentsInProject);
        
        await _meetingService.DeleteMeetingsForProject(projectId);

        var result = await base.TryRemoveAsync(projectId);
        if (result)
        {
            return new ServiceActionResult
            {
                Completed = true,
                Comment = $"Project with ID {projectId} deleted."
            };
        }

        return new ServiceActionResult
        {
            Completed = false,
            Comment = $"Failed to delete project with ID {projectId}"
        };
    }
}