using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.ProjectApplications.Requests;
using AlphaProjectManager.Controllers.ProjectApplications.Responses;
using AlphaProjectManager.Controllers.Shared;
using Application.DataQuery;
using Application.Services;
using Application.Services.TelegramBot.Notifier;
using Application.Utils;
using Domain.Entities;
using Domain.Entities.TelegramBot;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.ProjectApplications;

[Route("api/applications")]
public class ApplicationController : ControllerBase
{
    private readonly BaseService<ProjectApplication> _applicationService;
    private readonly BaseService<ApplicationQuestionAnswer> _answerService;
    private readonly BaseService<ApplicationMessage> _messagesService;
    private readonly ITelegramNotifier _telegramNotifier;

    public ApplicationController(BaseService<ProjectApplication> applicationService, BaseService<ApplicationQuestionAnswer> answerService,
        BaseService<ApplicationMessage> messagesService, ITelegramNotifier telegramNotifier)
    {
        _applicationService = applicationService;
        _answerService = answerService;
        _messagesService = messagesService;
        _telegramNotifier = telegramNotifier;
    }
    
    /// <summary>
    /// Получить краткую информацию о всех заявках на кейсы
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApplicationBriefListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplications([FromQuery] ApplicationStatus? status)
    {
        var foundApplications = await _applicationService.GetAsync(new DataQueryParams<ProjectApplication>
        {
            Expression = status.HasValue ? appl => appl.Status == status : null,
            IncludeParams = new IncludeParams<ProjectApplication>
            {
                IncludeProperties = [appl => appl.ProjectCase]
            }
        });
        
        return Ok(new ApplicationBriefListResponse
        {
            Applications = foundApplications.Select(ApplicationBriefResponse.FromApplication).ToArray()
        });
    }
    
    /// <summary>
    /// Получить полную информацию о заявке по id
    /// </summary>
    [HttpGet($"{{{nameof(applicationId)}:guid}}")]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplicationById([FromRoute] Guid applicationId)
    {
        var foundApplications = await _applicationService.GetAsync(new DataQueryParams<ProjectApplication>
        {
            Expression = appl => appl.Id == applicationId
        });
        if (foundApplications.Length == 0)
        {
            return SharedResponses.NotFoundObjectResponse<ProjectApplication>(applicationId);
        }

        var foundApplication = foundApplications[0];
        var msgs = await _messagesService.GetAsync(new DataQueryParams<ApplicationMessage>
        {
            Expression = msg => msg.ApplicationId == applicationId
        });
        foreach (var justReadMsg in msgs.Where(msg => !msg.IsRead))
        {
            justReadMsg.IsRead = true;
            await _messagesService.UpdateAsync(justReadMsg);
        }
        var answers = await _answerService.GetAsync(new DataQueryParams<ApplicationQuestionAnswer>
        {
            Expression = answer => answer.ApplicationId == applicationId
        });
        
        return Ok(ApplicationResponse.FromDomainEntities(foundApplication, answers, msgs));
    }
    
    /// <summary>
    /// Удалить заявку по id
    /// </summary>
    [HttpDelete($"{{{nameof(applicationId)}:guid}}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteApplicationById([FromRoute] Guid applicationId)
    {
        var foundApplications = await _applicationService.GetAsync(new DataQueryParams<ProjectApplication>
        {
            Expression = appl => appl.Id == applicationId
        });
        if (foundApplications.Length == 0)
        {
            return SharedResponses.NotFoundObjectResponse<ProjectApplication>(applicationId);
        }
        var msgs = await _messagesService.GetAsync(new DataQueryParams<ApplicationMessage>
        {
            Expression = msg => msg.ApplicationId == applicationId
        });
        var answers = await _answerService.GetAsync(new DataQueryParams<ApplicationQuestionAnswer>
        {
            Expression = answer => answer.ApplicationId == applicationId
        });
        await _messagesService.RemoveRangeAsync(msgs);
        await _answerService.RemoveRangeAsync(answers);
        if (await _applicationService.TryRemoveAsync(applicationId))
        {
            return SharedResponses.SuccessRequest();
        }
        return SharedResponses.FailedRequest("Failed to remove ProjectApplication from database");
    }
    
    /// <summary>
    /// Отправить сообщение по заявке с указанным id
    /// </summary>
    [HttpPost($"{{{nameof(applicationId)}:guid}}/send-message")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendMessageForApplication([FromRoute] Guid applicationId, [FromBody] SendMessageRequest dto)
    {
        var application = await _applicationService.GetByIdOrDefaultAsync(applicationId);
        if (application == null)
        {
            return SharedResponses.NotFoundObjectResponse<ProjectApplication>(applicationId);
        }

        var msg = new ApplicationMessage
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            Content = dto.Content,
            Direction = ApplicationMsgDirection.ToStudents,
            Timestamp = DateTime.Now.ConvertToTimestamp(),
            IsRead = true
        };
        await _messagesService.CreateAsync(msg);
        await _telegramNotifier.SendMsgFromTutorsAsync(application.ChatId, dto.Content);
        
        return Ok(new BaseStatusResponse
        {
            Completed = true,
            Message = ""
        });
    }
}