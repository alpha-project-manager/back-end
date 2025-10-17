using AlphaProjectManager.Controllers.ApplicationQuestions.Requests;
using AlphaProjectManager.Controllers.ApplicationQuestions.Responses;
using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.ProjectCases.Responses;
using AlphaProjectManager.Controllers.Shared;
using AlphaProjectManager.Controllers.Utility;
using Application.DataQuery;
using Application.Services;
using Domain.Entities.TelegramBot;
using Microsoft.AspNetCore.Mvc;
namespace AlphaProjectManager.Controllers.ApplicationQuestions;

[Route("/api/applications/questions")]
public class ApplicationQuestionController : ControllerBase
{
    private readonly BaseService<ApplicationQuestion> _questionService;

    public ApplicationQuestionController(BaseService<ApplicationQuestion> questionService)
    {
        _questionService = questionService;
    }
    
    /// <summary>
    /// Получить все вопросы для заявки
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(QuestionListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var foundQuestions = await _questionService.GetAsync(new DataQueryParams<ApplicationQuestion>());
        if (foundQuestions.Length == 0)
        {
            return Ok(new QuestionListResponse
            {
                Completed = true,
                Message = "",
                Questions = []
            });
        }
        var currentQuestion = foundQuestions.First(q => q.PrevQuestionId == null);
        var resultList = new List<ApplicationQuestion>() {currentQuestion};
        while (currentQuestion.NextQuestionId != null)
        {
            currentQuestion = foundQuestions.First(q => q.Id == currentQuestion.NextQuestionId);
            resultList.Add(currentQuestion);
        }
        return Ok(new QuestionListResponse
        {
            Completed = true,
            Message = "",
            Questions = resultList.Select(DtoConverter.ApplicationQuestionToResponse).ToArray()
        });
    }
    
    /// <summary>
    /// Получить вопрос для заявки по id
    /// </summary>
    [HttpGet("{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid questionId)
    {
        var foundQuestion = await _questionService.GetByIdOrDefaultAsync(questionId);
        if (foundQuestion == null)
        {
            return SharedResponses.NotFoundObjectResponse<ApplicationQuestion>(questionId);
        }
        return Ok(DtoConverter.ApplicationQuestionToResponse(foundQuestion));
    }
    
    /// <summary>
    /// Создать новый вопрос для заявки
    /// </summary>
    [HttpPost()]
    [ProducesResponseType(typeof(ProjectCaseFullResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateNewQuestion()
    {
        var lastQuestions = await _questionService.GetAsync(new DataQueryParams<ApplicationQuestion>
        {
            Expression = q => q.NextQuestionId == null
        });
        var newQuestion = new ApplicationQuestion
        {
            Id = Guid.NewGuid(),
            Title = "Члены команды",
            MsgText = "Введите информацию о составе вашей команды:",
            PrevQuestionId = lastQuestions.Length == 0 ? null : lastQuestions[0].Id,
        };
        await _questionService.CreateAsync(newQuestion);
        if (lastQuestions.Length != 0)
        {
            lastQuestions[0].NextQuestionId = newQuestion.Id;
            await _questionService.UpdateAsync(lastQuestions[0]);
        }
        
        return Ok(DtoConverter.ApplicationQuestionToResponse(newQuestion));
    }
    
    /// <summary>
    /// Удалить вопрос по id
    /// </summary>
    [HttpDelete("{questionId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion([FromRoute] Guid questionId)
    {
        var foundQuestions = await _questionService.GetAsync(new DataQueryParams<ApplicationQuestion>
        {
            Expression = q => q.Id == questionId,
            IncludeParams = new IncludeParams<ApplicationQuestion>
            {
                IncludeProperties = [q => q.NextQuestion, q => q.PrevQuestion]
            }
        });
        if (foundQuestions.Length == 0)
        {
            return SharedResponses.NotFoundObjectResponse<ApplicationQuestion>(questionId);
        }

        var foundQuestion = foundQuestions[0];
        if (foundQuestion.PrevQuestion != null)
        {
            foundQuestion.PrevQuestion.NextQuestionId = foundQuestion.NextQuestionId;
            await _questionService.UpdateAsync(foundQuestion.PrevQuestion);
        }
        if (foundQuestion.NextQuestion != null)
        {
            foundQuestion.NextQuestion.PrevQuestionId = foundQuestion.PrevQuestionId;
            await _questionService.UpdateAsync(foundQuestion.NextQuestion);
        }

        await _questionService.TryRemoveAsync(foundQuestion.Id);
        return Ok(new BaseStatusResponse
        {
            Completed = true,
            Message = ""
        });
    }
    
    /// <summary>
    /// Обновить данные вопроса для заявки
    /// </summary>
    [HttpPut("{questionId:guid}")]
    [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateQuestion([FromRoute] Guid questionId, [FromBody] UpdateQuestionRequest dto)
    {
        var foundQuestion = await _questionService.GetByIdOrDefaultAsync(questionId);
        if (foundQuestion == null)
        {
            return SharedResponses.NotFoundObjectResponse<Domain.Entities.ProjectCase>(questionId);
        }
        DtoConverter.MapPropertiesValues(dto, foundQuestion);
        await _questionService.UpdateAsync(foundQuestion);
        return Ok(DtoConverter.ApplicationQuestionToResponse(foundQuestion));
    }
}