using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.Projects.ControlPointsInProject;

public class ControlPointsInProjectController : ControllerBase
{
    private readonly ProjectsService _projectsService;
    private readonly BaseService<ControlPointInProject> _pointService;

    public ControlPointsInProjectController(ProjectsService projectsService, BaseService<ControlPointInProject> pointService)
    {
        _projectsService = projectsService;
        _pointService = pointService;
    }
    
    
}