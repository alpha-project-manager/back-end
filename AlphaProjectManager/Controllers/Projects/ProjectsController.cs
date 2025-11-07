using AlphaProjectManager.Controllers.Projects.Responses;
using AlphaProjectManager.Controllers.Students.Responses;
using Application.DataQuery;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaProjectManager.Controllers.Projects;

[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly BaseService<Project> _projectService;
    private readonly BaseService<StudentInProject> _studentsInProjectService;
    private readonly BaseService<Meeting> _meetingService;
    private readonly BaseService<ControlPointInProject> _controlPointService;

    public ProjectsController(BaseService<Project> projectService, BaseService<StudentInProject> studentsInProjectService,
        BaseService<Meeting> meetingService, BaseService<ControlPointInProject> controlPointService)
    {
        _projectService = projectService;
        _studentsInProjectService = studentsInProjectService;
        _meetingService = meetingService;
        _controlPointService = controlPointService;
    }
    
    /// <summary>
    /// Получить список проектов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProjectBriefListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectsBrief([FromQuery] int? skip, [FromQuery] int? take, 
        [FromQuery] Guid? tutorId, [FromQuery] Guid? studentId, [FromQuery] string? search)
    {
        var query = new DataQueryParams<Project>
        {
            Filters = [],
            Paging = new PagingParams
            {
                Skip = skip ?? 0,
                Take = take ?? 50
            },
            IncludeParams = new IncludeParams<Project>
            {
                IncludeProperties = [p => p.Tutor]
            }
        };
        if (tutorId.HasValue)
        {
            query.Filters.Add(p => p.TutorId == tutorId);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Expression = p => EF.Functions.ILike(p.Title, $"%{search}%") 
                                    || EF.Functions.ILike(p.Description, $"%{search}%");
        }
        if (studentId.HasValue)
        {
            var projectIds = (await _studentsInProjectService.GetAsync(new DataQueryParams<StudentInProject>
            {
                Expression = s => s.StudentId == studentId
            })).Select(f => f.ProjectId).ToArray();
            query.Filters.Add(pr => projectIds.Contains(pr.Id));
        }
        var projects = await _projectService.GetAsync(query);
        
        return Ok(new ProjectBriefListResponse
        {
            Projects = projects.Select(ProjectBriefResponse.FromProject).ToArray()
        });
    }
}