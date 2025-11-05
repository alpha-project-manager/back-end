using AlphaProjectManager.Controllers.ApplicationMessages.Requests;
using AlphaProjectManager.Controllers.ApplicationMessages.Responses;
using AlphaProjectManager.Controllers.Base.Responses;
using Application.DataQuery;
using Application.Services;
using Application.Services.TelegramBot.Notifier;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.Enums;

namespace AlphaProjectManager.Controllers.ApplicationMessages;

[Route("/api/application-messages")]
public class ApplicationMessageController : ControllerBase
{
    private readonly BaseService<ProjectApplication> _applicationService;
    private readonly BaseService<ApplicationMessage> _messageService;
    private readonly ITelegramNotifier _telegramNotifier;

    public ApplicationMessageController(BaseService<ProjectApplication> applicationService, 
        BaseService<ApplicationMessage> messageService, ITelegramNotifier telegramNotifier)
    {
        _applicationService = applicationService;
        _messageService = messageService;
        _telegramNotifier = telegramNotifier;
    }
    
    /// <summary>
    /// Получить все сообщения по заявке с указанным id
    /// </summary>
    [HttpGet("/api/application-messages/{applicationId:guid}")]
    [ProducesResponseType(typeof(MessageListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMessagesForApplication([FromRoute] Guid applicationId)
    {
        var foundMsgs = await _messageService.GetAsync(new DataQueryParams<ApplicationMessage>
        {
            Expression = m => m.ApplicationId == applicationId,
            Sorting = new SortingParams<ApplicationMessage>
            {
                OrderBy = m => m.Timestamp,
                Ascending = true
            }
        });
        foreach (var justReadMsg in foundMsgs.Where(msg => !msg.IsRead))
        {
            justReadMsg.IsRead = true;
            await _messageService.UpdateAsync(justReadMsg);
        }
        return Ok(new MessageListResponse
        {
            Messages = foundMsgs.Select(MessageResponse.FromApplicationMessage).ToArray()
        });
    }
    
    /// <summary>
    /// Отправить сообщение по заявке с указанным id
    /// </summary>
    [HttpPost("/api/application-messages/{applicationId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessageForApplication([FromRoute] Guid applicationId, [FromBody] SendMessageRequest dto)
    {
        var application = await _applicationService.GetByIdOrDefaultAsync(applicationId);
        if (application == null)
        {
            return Shared.SharedResponses.NotFoundObjectResponse<ProjectApplication>(applicationId);
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
        await _messageService.CreateAsync(msg);
        await _telegramNotifier.SendMsgFromTutorsAsync(application.ChatId, dto.Content);
        
        return Ok(new BaseStatusResponse
        {
            Completed = true,
            Message = ""
        });
    }
}