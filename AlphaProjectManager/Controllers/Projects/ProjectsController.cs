using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.Projects.Requests;
using AlphaProjectManager.Controllers.Projects.Responses;
using AlphaProjectManager.Controllers.Shared;
using Application.DataQuery;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaProjectManager.Controllers.Projects;

[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectsService _projectService;
    private readonly BaseService<StudentInProject> _studentsInProjectService;
    private readonly MeetingService _meetingService;
    private readonly BaseService<ControlPointInProject> _controlPointService;
    private readonly TeamProProjectImporter _teamProProjectImporter;
    private readonly BaseService<Student> _studentService;

    public ProjectsController(ProjectsService projectService, BaseService<StudentInProject> studentsInProjectService,
        MeetingService meetingService, BaseService<ControlPointInProject> controlPointService, TeamProProjectImporter teamProProjectImporter,
        BaseService<Student> studentService)
    {
        _projectService = projectService;
        _studentsInProjectService = studentsInProjectService;
        _meetingService = meetingService;
        _controlPointService = controlPointService;
        _teamProProjectImporter = teamProProjectImporter;
        _studentService = studentService;
    }
    
    /// <summary>
    /// Получить список проектов с краткой информацией
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProjectBriefListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectsBriefList([FromQuery] int? skip, [FromQuery] int? take, 
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
    
    /// <summary>
    /// Импортировать список проектов из Team Project
    /// </summary>
    [HttpPost("import-from-team-pro")]
    [ProducesResponseType(typeof(ImportProjectsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ImportProjectsFromTeamPro([FromBody] ImportProjectsFromTeamProRequest dto)
    {
        var result = await _teamProProjectImporter.ImportProjectsFromTeamPro(dto.Login, dto.Password);
        return Ok(new ImportProjectsResponse
        {
            CreatedProjects = result.CreatedProjects.Select(ProjectBriefResponse.FromProject).ToList(),
            UpdatedProjects = result.UpdatedProjects.Select(ProjectBriefResponse.FromProject).ToList(),
            Completed = result.Completed,
            Message = result.Comment
        });
    }
    
    /// <summary>
    /// Создать новый проект
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectBriefResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateNewProject([FromBody] CreateNewProjectRequest dto)
    {
        var sem = CurrentSemesterHelper.GetCurrentSemester();
        var project = new Project
        {
            Id = Guid.NewGuid(),
            CaseId = dto.CaseId,
            TeamTitle = "",
            Title = dto.Title,
            Description = "",
            TutorId = null, // TODO: From Current User
            MeetingUrl = "",
            Status = ProjectStatus.Created,
            Semester = sem.Semester,
            AcademicYear = sem.Year
        };
        await _projectService.CreateAsync(project);
        return Ok(ProjectBriefResponse.FromProject(project));
    }
    
    /// <summary>
    /// Удалить проект
    /// </summary>
    [HttpDelete("{projectId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId)
    {
        var result = await _projectService.DeleteProject(projectId);
        if (!result.Completed)
        {
            return SharedResponses.FailedRequest(result.Comment);
        }
        return Ok(new BaseStatusResponse
        {
            Completed = true,
            Message = result.Comment
        });
    }
    
    /// <summary>
    /// Получить полную информацию о проекте
    /// </summary>
    [HttpGet("{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectFullResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectFullInfo([FromRoute] Guid projectId)
    {
        var foundProjects  = await _projectService.GetAsync(new DataQueryParams<Project>
        {
            Expression = p => p.Id == projectId,
            IncludeParams = new IncludeParams<Project> { IncludeProperties = [p => p.Tutor] }
        });
        if (foundProjects.Length == 0)
        {
            return SharedResponses.NotFoundObjectResponse<Project>(projectId);
        }
        var project = foundProjects[0];
        
        var controlPoints = await _controlPointService.GetAsync(new DataQueryParams<ControlPointInProject>
        {
            Expression = p => p.ProjectId == projectId
        });
        var students = (await _studentsInProjectService.GetAsync(new DataQueryParams<StudentInProject>
        {
            Expression = s => s.ProjectId == projectId,
            IncludeParams = new IncludeParams<StudentInProject>() { IncludeProperties = [s => s.Student, s=> s.Student.Role] }
        })).Select(s => s.Student).ToArray();
        var meetingsDict = await _meetingService.GetMeetingsForProject(projectId);
        
        return Ok(ProjectFullResponse.FromDomainEntities(project, controlPoints, students, meetingsDict));
    }
    
    /// <summary>
    /// Обновить информацию о проекте
    /// </summary>
    [HttpPut("{projectId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject([FromRoute] Guid projectId, [FromBody] UpdateProjectRequest dto)
    {
        var project = await _projectService.GetByIdOrDefaultAsync(projectId);
        if (project == null)
        {
            return SharedResponses.NotFoundObjectResponse<Project>(projectId);
        }
        dto.ApplyToProject(project);
        await _projectService.UpdateAsync(project);
        return Ok(new BaseStatusResponse
        {
            Completed = true,
            Message = ""
        });
    }
    
    /// <summary>
    /// Удалить студента из проекта
    /// </summary>
    [HttpDelete("{projectId:guid}/students/{studentId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteStudentFromProject([FromRoute] Guid projectId, [FromRoute] Guid studentId)
    {
        var foundResult = await _studentsInProjectService.GetAsync(new DataQueryParams<StudentInProject>
        {
            Expression = s => s.StudentId == studentId && s.ProjectId == projectId
        });
        if (foundResult.Length == 0)
        {
            return SharedResponses.FailedRequest($"Student with ID {studentId} does not participate in project with ID {projectId}.");
        }
        await _studentsInProjectService.TryRemoveAsync(foundResult[0].Id);
        return Ok(new BaseStatusResponse
        {
            Completed = true,
            Message = "Student removed from project."
        });
    }
    
    /// <summary>
    /// Добавить студента в проекта
    /// </summary>
    [HttpPost("{projectId:guid}/students/{studentId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddStudentToProject([FromRoute] Guid projectId, [FromRoute] Guid studentId)
    {
        var foundResult = await _studentsInProjectService.GetAsync(new DataQueryParams<StudentInProject>
        {
            Expression = s => s.StudentId == studentId && s.ProjectId == projectId
        });
        if (foundResult.Length != 0)
        {
            return SharedResponses.FailedRequest($"Student with ID {studentId} already participate in project with ID {projectId}.");
        }
        var student = await _studentService.GetByIdOrDefaultAsync(studentId);
        if (student == null)
        {
            return SharedResponses.FailedRequest($"Student with ID {studentId} does not exist.");
        }
        var project = await _projectService.GetByIdOrDefaultAsync(projectId);
        if (project == null)
        {
            return SharedResponses.FailedRequest($"Project with ID {projectId} does not exist.");
        }
        await _studentsInProjectService.CreateAsync(new StudentInProject
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            StudentId = studentId
        });
        return Ok(new BaseStatusResponse
        {
            Completed = true,
            Message = "Student added to project."
        });
    }
}