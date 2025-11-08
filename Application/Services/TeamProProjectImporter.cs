using Application.DataQuery;
using Application.Models;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using TeamProjectConnection.Intefaces;
using TeamProjectConnection.Models.TeamProResponses.Catalog;

namespace Application.Services;

public class TeamProProjectImporter
{
    private readonly BaseService<Project> _projectService;
    private readonly ITeamProjectManager _teamProjectManager;
    private readonly BaseService<Tutor> _tutorService;
    private readonly BaseService<Student> _studentService;
    private readonly BaseService<StudentInProject> _studentInProjectService;
    private readonly BaseService<ProjectCase> _caseService;
    private readonly BaseService<ControlPoint> _controlPointService;
    private readonly BaseService<ControlPointInProject> _controlPointInProjectService;

    public TeamProProjectImporter(BaseService<Project> projectService, ITeamProjectManager teamProjectManager,
        BaseService<Tutor> tutorService, BaseService<Student> studentService, BaseService<StudentInProject> studentInProjectService,
        BaseService<ProjectCase> caseService, BaseService<ControlPoint> controlPointService, 
        BaseService<ControlPointInProject> controlPointInProjectService)
    {
        _projectService = projectService;
        _teamProjectManager = teamProjectManager;
        _tutorService = tutorService;
        _studentService = studentService;
        _studentInProjectService = studentInProjectService;
        _caseService = caseService;
        _controlPointService = controlPointService;
        _controlPointInProjectService = controlPointInProjectService;
    }
    
    public async Task<ImportProjectsActionResult> ImportProjectsFromTeamPro(string login, string password)
    {
        if (!_teamProjectManager.AuthManager.IsAuthorized())
        {
            var authorized = await _teamProjectManager.AuthManager.TryAuthAsync(login, password);
            if (!authorized)
            {
                return FailedResult("Failed to authorize to Team Project.");
            }
        }
        var period = await _teamProjectManager.GetCurrentPeriod();
        if (period == null)
        {
            return FailedResult("Failed to get current period from Team Project.");
        }
        var projects = await _teamProjectManager.GetProjectsForPeriod(period.Year, period.Term);
        if (projects == null)
        {
            return FailedResult($"Failed to get projects for {period.Year} year and {period.Term} semester from Team Project.");
        }

        var result = new ImportProjectsActionResult
        {
            Completed = true,
            Comment = ""
        };

        foreach (var importedProject in projects.Items)
        {
            var (justCreated, project)= await ImportProjectAsync(importedProject);
            // await TryImportTutorByFullName(project, importedProject.MainCurator?.Fullname ?? "");
            await TryImportDetailsToProjectAsync(project);
            await TryImportStudentsAndTutorAsync(project);
            await CheckOrCreateControlPointsInProjectAsync(project);
            
            await _projectService.UpdateAsync(project);
            if (justCreated)
            {
                result.CreatedProjects.Add(project);
            }
            else
            {
                result.UpdatedProjects.Add(project);
            }
        }

        return result;
    }

    private async Task CheckOrCreateControlPointsInProjectAsync(Project project)
    {
        var controlPoints = await _controlPointService.GetAsync(new DataQueryParams<ControlPoint>());
        foreach (var point in controlPoints)
        {
            var existingPointInProject = await _controlPointInProjectService.GetAsync(
                new DataQueryParams<ControlPointInProject>
                {
                    Expression = p => p.ControlPointId == point.Id && p.ProjectId == project.Id
                });
            if (existingPointInProject.Length > 0)
            {
                continue;
            }

            var pointInProject = new ControlPointInProject
            {
                Id = Guid.NewGuid(),
                ControlPointId = point.Id,
                ProjectId = project.Id,
                Title = point.Title,
                VideoUrl = "",
                CompanyMark = 0,
                UrfuMark = 0,
                Completed = false,
                Date = point.Date,
                HasMarkInTeamPro = false
            };
            await _controlPointInProjectService.CreateAsync(pointInProject);
        }
    }
    
    private async Task<(bool justCreated, Project project)> ImportProjectAsync(TeamProProjectResponse projectResponse)
    {
        var project = await _projectService.GetByIdOrDefaultAsync(projectResponse.Id);
        if (project != null)
        {
            return (false, project);
        }
        project = new Project
        {
            Id = projectResponse.Id,
            CaseId = null,
            TeamTitle = "",
            Title = projectResponse.Title + " " + projectResponse.PassportNumber,
            Description = "",
            TutorId = null,
            MeetingUrl = "",
            Status = ProjectStatus.InWork,
            Semester = Semester.Autumn,
            AcademicYear = DateTime.Now.Year
        };
        await _projectService.CreateAsync(project);

        return (true, project);
    }
    
    private async Task TryImportDetailsToProjectAsync(Project project)
    {
        var details = await _teamProjectManager.GetDetailsForProject(project.Id);
        if (details == null)
        {
            return;
        }
        project.Description = details.Description;
        project.Semester = details.Period.Term == 1 ? Semester.Autumn : Semester.Spring;
        project.AcademicYear = details.Period.Year;
        
        if (!project.CaseId.HasValue)
        {
            var supposedCases = await _caseService.GetAsync(new DataQueryParams<ProjectCase>
            {
                Expression = c => EF.Functions.ILike(c.Title, $"%{details.Title}%")
            });
            if (supposedCases.Length > 0)
            {
                project.CaseId = supposedCases[0].Id;
            }
        }
    }
    
    private async Task TryImportStudentsAndTutorAsync(Project project)
    {
        var teamResponse = await _teamProjectManager.GetTeamForProject(project.Id);
        if (teamResponse == null)
        {
            return;
        }
        foreach (var studentMember in teamResponse.Students)
        {
            var guidId = GuidExtensions.FromInt(studentMember.Id);
            var student = await _studentService.GetByIdOrDefaultAsync(guidId);
                    
            if (student == null)
            {
                var nameParts = studentMember.Fullname.Split();
                student = new Student
                {
                    Id = guidId,
                    LastName = nameParts.Length > 0 ? nameParts[0] : "",
                    FirstName = nameParts.Length > 1 ? nameParts[1] : "",
                    Patronymic = nameParts.Length > 2 ? nameParts[2] : "",
                    AcademicGroup = "",
                    RoleId = null
                };
                await _studentService.CreateAsync(student);
            }
            await CheckOrCreateStudentInProjectAsync(project, student);
        }

        if (teamResponse.MainCurator != null)
        {
            var foundTutors = await _tutorService.GetAsync(new DataQueryParams<Tutor>
            {
                Expression = t => EF.Functions.ILike(t.FullName, $"%{teamResponse.MainCurator.Fullname}%")
            });
            if (foundTutors.Length > 0)
            {
                project.TutorId = foundTutors[0].Id;
            }
            else
            {
                var nameParts = teamResponse.MainCurator.Fullname.Split();
                var tutor = new Tutor
                {
                    Id = GuidExtensions.FromInt(teamResponse.MainCurator.Id),
                    LastName = nameParts.Length > 0 ? nameParts[0] : "",
                    FirstName = nameParts.Length > 1 ? nameParts[1] : "",
                    Patronymic = nameParts.Length > 2 ? nameParts[2] : "",
                };
                await _tutorService.CreateAsync(tutor);
            }
        }
        
    }

    private async Task CheckOrCreateStudentInProjectAsync(Project project, Student student)
    {
        var foundStudentsInProject = await _studentInProjectService.GetAsync(new DataQueryParams<StudentInProject>
        {
            Expression = s => s.StudentId == student.Id && s.ProjectId == project.Id
        });
        if (foundStudentsInProject.Length == 0)
        {
            var studentInProject = new StudentInProject
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                StudentId = student.Id
            };
            await _studentInProjectService.CreateAsync(studentInProject);
        }
    }
    
    private ImportProjectsActionResult FailedResult(string comment)
    {
        return new ImportProjectsActionResult
        {
            Completed = false,
            Comment = comment
        };
    }
}