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
using Microsoft.EntityFrameworkCore;

namespace AlphaProjectManager.Controllers.ProjectCases;

[Route("/api/project-cases")]
public class ProjectCaseController : ControllerBase
{
    private readonly BaseService<ProjectCase> _caseService;
    private readonly BaseService<CaseVote> _votesService;

    public ProjectCaseController(BaseService<ProjectCase> caseService, BaseService<CaseVote> votesService)
    {
        _caseService = caseService;
        _votesService = votesService;
    }
    
    /// <summary>
    /// Получить краткую информацию о всех кейсах
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProjectCaseListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBrief([FromQuery] int? skip, [FromQuery] int? take, 
        [FromQuery] Guid? tutorId, [FromQuery] string? search)
    {
        var query = new DataQueryParams<ProjectCase>
        {
            Filters = [],
            Paging = new PagingParams(skip ?? 0, take ?? 10),
            IncludeParams = new IncludeParams<ProjectCase>
            {
                IncludeProperties = [p => p.Tutor]
            },
            Sorting = new SortingParams<ProjectCase>
            {
                OrderBy = c => c.UpdatedTime,
                Ascending = false
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
        var foundCases = await _caseService.GetAsync(query);
        var caseIds = foundCases.Select(c => c.Id).ToArray();
        var caseVotesMap = (await _votesService.GetAsync(new DataQueryParams<CaseVote>
        {
            Expression = v => caseIds.Contains(v.CaseId)
        })).GroupBy(v => v.CaseId)
        .ToDictionary(
            g => foundCases.First(a => a.Id == g.Key), 
            g => g.ToArray());
        
        return Ok(new ProjectCaseListResponse
        {
            Cases = caseVotesMap.Select(kv => ProjectCaseBriefResponse.FromProjectCase(kv.Key, kv.Value)).ToArray(),
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

        var foundCase = foundCases[0];
        var votes = await _votesService.GetAsync(new DataQueryParams<CaseVote>
        {
            Expression = v => v.CaseId == foundCase.Id
        });
        return Ok(ProjectCaseBriefResponse.FromProjectCase(foundCase, votes));
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
        return Ok(ProjectCaseFullResponse.FromProjectCase(foundCases[0]));
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
            IsActive = false,
            UpdatedTime = DateTime.Now.ToUniversalTime()
        };
        await _caseService.CreateAsync(newCase);
        
        return Ok(ProjectCaseFullResponse.FromProjectCase(newCase));
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
        foundCase.UpdatedTime = DateTime.Now.ToUniversalTime();
        await _caseService.UpdateAsync(foundCase);
        return Ok(ProjectCaseFullResponse.FromProjectCase(foundCase));
    }
}