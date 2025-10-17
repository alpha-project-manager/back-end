using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.ProjectCases.Requests;
using AlphaProjectManager.Controllers.ProjectCases.Responses;
using AlphaProjectManager.Controllers.Shared;
using AlphaProjectManager.Controllers.TestController.Responses;
using AlphaProjectManager.Controllers.Utility;
using Application.DataQuery;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.ProjectCases;

[Route("/api/project-cases")]
public class ProjectCaseController : ControllerBase
{
    private readonly BaseService<ProjectCase> _caseService;

    public ProjectCaseController(BaseService<ProjectCase> caseService)
    {
        _caseService = caseService;
    }
    
    /// <summary>
    /// Получить краткую информацию о всех кейсах
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProjectCaseListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBrief()
    {
        var foundCases = await _caseService.GetAsync(new DataQueryParams<ProjectCase>
        {
            IncludeParams = new IncludeParams<ProjectCase>
            {
                IncludeProperties = [c => c.Tutor]
            }
        });
        return Ok(new ProjectCaseListResponse
        {
            Cases = foundCases.Select(DtoConverter.ProjectCaseToBriefResponse).ToArray(),
            Completed = true,
            Message = ""
        });
    }
    
    /// <summary>
    /// Получить краткую информацию о кейсе по id
    /// </summary>
    [HttpGet("{caseId:guid}/brief")]
    [ProducesResponseType(typeof(ProjectCaseBriefResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBriefById([FromRoute] Guid caseId)
    {
        var foundCases = await _caseService.GetAsync(new DataQueryParams<ProjectCase>
        {
            Expression = c => c.Id == caseId,
            IncludeParams = new IncludeParams<ProjectCase>
            {
                IncludeProperties = [c => c.Tutor]
            }
        });
        if (foundCases.Length == 0)
        {
            return SharedResponses.NotFoundObjectResponse<ProjectCase>(caseId);
        }
        return Ok(DtoConverter.ProjectCaseToBriefResponse(foundCases[0]));
    }
    
    /// <summary>
    /// Получить полную информацию о кейсе по id
    /// </summary>
    [HttpGet("{caseId:guid}")]
    [ProducesResponseType(typeof(ProjectCaseFullResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid caseId)
    {
        var foundCases = await _caseService.GetAsync(new DataQueryParams<ProjectCase>
        {
            Expression = c => c.Id == caseId,
            IncludeParams = new IncludeParams<ProjectCase>
            {
                IncludeProperties = [c => c.Tutor]
            }
        });
        if (foundCases.Length == 0)
        {
            return SharedResponses.NotFoundObjectResponse<ProjectCase>(caseId);
        }
        return Ok(DtoConverter.ProjectCaseToFullResponse(foundCases[0]));
    }
    
    /// <summary>
    /// Создать новый кейс
    /// </summary>
    [HttpPost()]
    [ProducesResponseType(typeof(ProjectCaseFullResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateNewCase()
    {
        var newCase = new ProjectCase
        {
            Id = Guid.NewGuid(),
            Title = "Новый кейс",
            MaxTeams = 0,
            AcceptedTeams = 0,
            IsActive = false
        };
        await _caseService.CreateAsync(newCase);
        
        return Ok(DtoConverter.ProjectCaseToFullResponse(newCase));
    }
    
    /// <summary>
    /// Удалить кейс по id
    /// </summary>
    [HttpDelete("{caseId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCase([FromRoute] Guid caseId)
    {
        var result = await _caseService.TryRemoveAsync(caseId);
        if (!result)
        {
            return SharedResponses.NotFoundObjectResponse<ProjectCase>(caseId);
        }
        return Ok(new BaseStatusResponse
        {
            Completed = result,
            Message = ""
        });
    }
    
    /// <summary>
    /// Обновить информацию о кейсе
    /// </summary>
    [HttpPut("{caseId:guid}")]
    [ProducesResponseType(typeof(ProjectCaseFullResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCase([FromRoute] Guid caseId, [FromBody] UpdateCaseRequest dto)
    {
        var foundCase = await _caseService.GetByIdOrDefaultAsync(caseId);
        if (foundCase == null)
        {
            return SharedResponses.NotFoundObjectResponse<ProjectCase>(caseId);
        }
        DtoConverter.MapPropertiesValues(dto, foundCase);
        await _caseService.UpdateAsync(foundCase);
        return Ok(DtoConverter.ProjectCaseToFullResponse(foundCase));
    }
}