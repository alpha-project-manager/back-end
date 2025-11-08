using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.Projects.ControlPointsInProject.Requests;
using AlphaProjectManager.Controllers.Projects.ControlPointsInProject.Responses;
using AlphaProjectManager.Controllers.Shared;
using Application.DataQuery;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.Projects.ControlPointsInProject;

[Route("api/projects/{projectId:guid}/control-points")]
public class ControlPointsInProjectController : ControllerBase
{
    private readonly BaseService<ControlPointInProject> _pointService;

    public ControlPointsInProjectController(BaseService<ControlPointInProject> pointService)
    {
        _pointService = pointService;
    }
    
    /// <summary>
    /// Получить список контрольных точек проекта
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ControlPointProjectListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllControlPoints([FromRoute] Guid projectId)
    {
        var foundPoints = await _pointService.GetAsync(new DataQueryParams<ControlPointInProject>()
        {
            Expression = p => p.ProjectId == projectId,
            Sorting = new SortingParams<ControlPointInProject>
            {
                OrderBy = p => p.Date,
                Ascending = false
            }
        });
        return Ok(new ControlPointProjectListResponse
        {
            ControlPoints = foundPoints.Select(ControlPointProjectResponse.FromControlPoint).ToList()
        });
    }
    
    /// <summary>
    /// Создать новую контрольную точку в проекте
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ControlPointProjectResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateControlPoint([FromRoute] Guid projectId)
    {
        var point = new ControlPointInProject
        {
            Id = Guid.NewGuid(),
            ControlPointId = null,
            ProjectId = projectId,
            Title = "Контрольная точка",
            VideoUrl = "",
            CompanyMark = 0,
            UrfuMark = 0,
            Completed = false,
            Date = DateTime.Today,
            HasMarkInTeamPro = false
        };
        await _pointService.CreateAsync(point);
        
        return Ok(ControlPointProjectResponse.FromControlPoint(point));
    }
    
    /// <summary>
    /// Обновить информацию о контрольной точке в проекте
    /// </summary>
    [HttpPut("{pointId:guid}")]
    [ProducesResponseType(typeof(ControlPointProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateControlPoint([FromRoute] Guid projectId, [FromRoute] Guid pointId,
        [FromBody] UpdateControlPointInProjectRequest dto)
    {
        var point = await _pointService.GetByIdOrDefaultAsync(pointId);
        if (point == null)
        {
            return SharedResponses.NotFoundObjectResponse<ControlPointInProject>(pointId);
        }
        dto.ApplyToControlPointInProject(point);
        await _pointService.UpdateAsync(point);
        return Ok(ControlPointProjectResponse.FromControlPoint(point));
    }
    
    /// <summary>
    /// Удалить контрольную точку из проекта
    /// </summary>
    [HttpDelete("{pointId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteControlPoint([FromRoute] Guid projectId, [FromRoute] Guid pointId)
    {
        var completed = await _pointService.TryRemoveAsync(pointId);
        if (!completed)
        {
            return SharedResponses.NotFoundObjectResponse<ControlPointInProject>(pointId);
        }
        return Ok(new BaseStatusResponse
        {
            Completed = true,
            Message = "Control point removed from project."
        });
    }
}