using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.ControlPoints.Requests;
using AlphaProjectManager.Controllers.ControlPoints.Responses;
using AlphaProjectManager.Controllers.Shared;
using Application.DataQuery;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.ControlPoints;

[Route("api/control-points")]
public class ControlPointsController : ControllerBase
{
    private readonly ControlPointService _controlPointService;
    private readonly BaseService<ControlPointInProject> _pointInProjectService;

    public ControlPointsController(ControlPointService controlPointService, BaseService<ControlPointInProject> pointInProjectService)
    {
        _controlPointService = controlPointService;
        _pointInProjectService = pointInProjectService;
    }
    
    /// <summary>
    /// Получить список всех контрольных точек
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ControlPointListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllControlPoints()
    {
        var foundPoints = await _controlPointService.GetAsync(new DataQueryParams<ControlPoint>()
        {
            Sorting = new SortingParams<ControlPoint>
            {
                OrderBy = p => p.Date,
                Ascending = true
            }
        });
        return Ok(new ControlPointListResponse
        {
            ControlPoints = foundPoints.Select(ControlPointResponse.FromControlPoint).ToList()
        });
    }
    
    /// <summary>
    /// Создать новую контрольную точку
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ControlPointResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateControlPoint([FromBody] CreateControlPointRequest dto)
    {
        var result = await _controlPointService.CreateNewControlPoint(dto.CreateInAllProjects);
        return Ok(ControlPointResponse.FromControlPoint(result));
    }
    
    /// <summary>
    /// Обновить информацию о контрольной точке
    /// </summary>
    [HttpPut("{pointId:guid}")]
    [ProducesResponseType(typeof(ControlPointResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateControlPoint([FromRoute] Guid pointId, [FromBody] UpdateControlPointRequest dto)
    {
        var point = await _controlPointService.UpdateControlPoint(pointId, dto.Date, dto.Title, dto.UpdateInAllProjects);
        if (point == null)
        {
            return SharedResponses.NotFoundObjectResponse<ControlPoint>(pointId);
        }
        return Ok(ControlPointResponse.FromControlPoint(point));
    }
    
    /// <summary>
    /// Удалить контрольную точку
    /// </summary>
    [HttpDelete("{pointId:guid}")]
    [ProducesResponseType(typeof(ControlPointResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteControlPoint([FromRoute] Guid pointId, [FromBody] DeleteControlPointRequest dto)
    {
        var result = await _controlPointService.DeleteControlPoint(pointId, dto.DeleteInAllProjects);
        if (!result.Completed)
        {
            return SharedResponses.FailedRequest(result.Comment);
        }
        return Ok(BaseStatusResponse.FromServiceActionResult(result));
    }
}