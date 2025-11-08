using Application.DataQuery;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ControlPointService : BaseService<ControlPoint>
{
    private readonly BaseService<ControlPointInProject> _inProjectService;
    private readonly ProjectsService _projectsService;

    public ControlPointService(IDbContextFactory<ProjectManagerDbContext> dbContextFactory,
        BaseService<ControlPointInProject> inProjectService, ProjectsService projectsService) : base(dbContextFactory)
    {
        _inProjectService = inProjectService;
        _projectsService = projectsService;
    }

    public async Task<ControlPoint> CreateNewControlPoint(bool createInAllInWorkProjects)
    {
        var point = new ControlPoint
        {
            Id = Guid.NewGuid(),
            Title = "Контрольная точка",
            Date = DateTime.Today
        };
        await base.CreateAsync(point);
        if (createInAllInWorkProjects)
        {
            var projects = await _projectsService.GetAsync(new DataQueryParams<Project>
            {
                Expression = p => p.Status == ProjectStatus.InWork
            });
            foreach (var project in projects)
            {
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
                await _inProjectService.CreateAsync(pointInProject);
            }
        }

        return point;
    }
    
    public async Task<ControlPoint?> UpdateControlPoint(Guid pointId, DateOnly date, string? title, bool updateInAllProjects)
    {
        var point = await base.GetByIdOrDefaultAsync(pointId);
        if (point == null)
        {
            return null;
        }

        point.Date = date.ToDateTime(new TimeOnly());
        point.Title = title ?? "";
        await base.UpdateAsync(point);
        
        if (updateInAllProjects)
        {
            var controlPointInProjects = await _inProjectService.GetAsync(new DataQueryParams<ControlPointInProject>
            {
                Expression = p => p.ControlPointId == point.Id
            });
            
            foreach (var controlPointInProject in controlPointInProjects)
            {
                controlPointInProject.Date = point.Date;
                controlPointInProject.Title = point.Title;
                await _inProjectService.UpdateAsync(controlPointInProject);
            }
        }

        return point;
    }
    
    public async Task<ServiceActionResult> DeleteControlPoint(Guid pointId, bool deleteInAllProjects)
    {
        var controlPointInProjects = await _inProjectService.GetAsync(new DataQueryParams<ControlPointInProject>
        {
            Expression = p => p.ControlPointId == pointId
        });
        
        if (deleteInAllProjects)
        {
            await _inProjectService.RemoveRangeAsync(controlPointInProjects);
        }
        else
        {
            foreach (var controlPointInProject in controlPointInProjects)
            {
                controlPointInProject.ControlPointId = null;
                await _inProjectService.UpdateAsync(controlPointInProject);
            }
        }
        
        var result = await base.TryRemoveAsync(pointId);
        if (result)
        {
            return new ServiceActionResult
            {
                Completed = true,
                Comment = "Control point deleted"
            };
        }
        return ServiceActionResult.Failed($"Control point with provided ID {pointId} not found.");
    }
}