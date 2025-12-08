using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.Shared;
using AlphaProjectManager.Controllers.Tutors.Requests;
using AlphaProjectManager.Controllers.Tutors.Responses;
using Application.DataQuery;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaProjectManager.Controllers.Tutors;

[Route("/api/tutors")]
public class TutorsController : ControllerBase
{
    private readonly BaseService<Tutor> _tutorService;

    public TutorsController(BaseService<Tutor> tutorService)
    {
        _tutorService = tutorService;
    }
    
    /// <summary>
    /// Создать нового куратора
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateNewTutor([FromBody] CreateNewTutorRequest dto)
    {
        var tutor = dto.ToTutorEntity();
        await _tutorService.CreateAsync(tutor);
        return Ok(TutorResponse.FromTutor(tutor));
    }
    
    /// <summary>
    /// Получить список кураторов в системе
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TutorsListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFiltered([FromQuery] int? skip, [FromQuery] int? take, [FromQuery] string? search)
    {
        var query = new DataQueryParams<Tutor>
        {
            Paging = new PagingParams
            {
                Skip = skip ?? 0,
                Take = take ?? 10
            }
        };
        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Expression = t => EF.Functions.ILike(t.FullName, $"%{search}%");
        }
        var foundTutors = await _tutorService.GetAsync(query);
        return Ok(new TutorsListResponse
        {
            Tutors = foundTutors.Select(TutorResponse.FromTutor).ToList()
        });
    }
    
    /// <summary>
    /// Получить информацию о кураторе по Id
    /// </summary>
    [HttpGet("{tutorId:guid}")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailsById([FromRoute] Guid tutorId)
    {
        var foundTutor = await _tutorService.GetByIdOrDefaultAsync(tutorId);
        if (foundTutor == null)
        {
            return SharedResponses.NotFoundObjectResponse<Tutor>(tutorId);
        }
        return Ok(TutorResponse.FromTutor(foundTutor));
    }
    
    /// <summary>
    /// Обновить информацию о кураторе по Id
    /// </summary>
    [HttpPut("{tutorId:guid}")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDetailsById([FromRoute] Guid tutorId, [FromBody] UpdateTutorRequest dto)
    {
        var foundTutor = await _tutorService.GetByIdOrDefaultAsync(tutorId);
        if (foundTutor == null)
        {
            return SharedResponses.NotFoundObjectResponse<Tutor>(tutorId);
        }
        dto.ApplyToTutor(foundTutor);
        await _tutorService.UpdateAsync(foundTutor);
        
        return Ok(TutorResponse.FromTutor(foundTutor));
    }
    
    /// <summary>
    /// Удалить куратора
    /// </summary>
    [HttpDelete("{tutorId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTutor([FromRoute] Guid tutorId)
    {
        var completed = await _tutorService.TryRemoveAsync(tutorId);
        if (completed)
        {
            return SharedResponses.SuccessRequest("Tutor deleted.");
        }
        return SharedResponses.NotFoundObjectResponse<TutorResponse>(tutorId);
    }
}