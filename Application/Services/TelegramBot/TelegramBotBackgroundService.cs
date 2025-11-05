using Application.Utils;
using Domain.Entities;
using Domain.Entities.TelegramBot;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Services.TelegramBot;

public partial class TelegramBotBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotBackgroundService> _logger;
    
    public TelegramBotBackgroundService(
        ITelegramBotClient botClient, ILogger<TelegramBotBackgroundService> logger, IServiceProvider services)
    {
        _services = services;
        _botClient = botClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Запуск Telegram бота...");

        var me = await _botClient.GetMe(stoppingToken);
        _logger.LogInformation("Бот {username} успешно инициализирован.", me.Username);

        var options = new ReceiverOptions { AllowedUpdates = [] };

        try
        {
            await _botClient.ReceiveAsync(HandleUpdateAsync, HandleErrorAsync, options, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Остановка Telegram бота по токену отмены.");
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { Text: { } messageText })
        {
            await HandleMessageReceived(update.Message);
            return;
        }
        if (update.CallbackQuery?.Data != null)
        {
            await HandleCallbackQueryReceived(update, update.CallbackQuery.Data);
        }
    }

    private async Task HandleCallbackQueryReceived(Update update, string callbackQueryData)
    {
        if (callbackQueryData.StartsWith("Reset"))
        {
            await HandleResetCallbackReceived(update, callbackQueryData);
            return;
        }
        if (update.CallbackQuery is { Message: not null })
        {
            var application = await TryGetApplicationByChatIdAsync(update.CallbackQuery.Message.Chat.Id);
            if (application != null)
            {
                await _botClient.EditMessageReplyMarkup(application.ChatId, update.CallbackQuery.Message!.Id, replyMarkup: null);
                var text = GetTextForExistingApplication(application);
                await _botClient.SendMessage(application.ChatId, text);
                return;
            }
        }
        
        if (callbackQueryData.StartsWith("SelectProjectCase="))
        {
            var caseId = Guid.Parse(update.CallbackQuery!.Data!.Split("=")[1]);
            await OnSelectProjectUpdateReceivedAsync(update, caseId);
        }
        else if (callbackQueryData.StartsWith("BackToProjectList"))
        {
            await _botClient.DeleteMessage(update.CallbackQuery!.Message!.Chat.Id, update.CallbackQuery.Message!.Id);
            await SendSelectProjectCaseMenu(update.CallbackQuery.Message!.Chat.Id);
        }
        else if (callbackQueryData.StartsWith("ConfirmSelectProjectCase="))
        {
            var caseId = Guid.Parse(callbackQueryData.Split("=")[1]);
            await OnConfirmProjectUpdateReceivedAsync(update, caseId);
        }
    }
    
    private async Task HandleMessageReceived(Message message)
    {
        var text = message.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
        
        var application = await TryGetApplicationByChatIdAsync(message.Chat.Id);
        if (text == "/reset" && application != null)
        {
            await HandleResetCommandReceived(application, message);
            return;
        }
        if (text == "/reset")
        {
            await _botClient.SendMessage(message.Chat.Id, "В системе нет заявки от вас.");
        }

        if (application == null)
        {
            await SendSelectProjectCaseMenu(message.Chat.Id);
            return;
        }

        if (application.Status == ApplicationStatus.Rejected)
        {
            await _botClient.SendMessage(message.Chat.Id, "К сожалению, ваша заявка была отклонена.");
            return;
        }
        if (string.IsNullOrWhiteSpace(application.TeamTitle))
        {
            await ApplicationTeamTitleAnswered(application, text);
            await SendQuestionForApplicationAsync(application);
            return;
        }
        
        if (application.Status == ApplicationStatus.InProgress)
        {
            await ApplicationQuestionAnswered(application, message);
            if (application.Status == ApplicationStatus.New)
            {
                await SendThanksFinishedApplicationAsync(application);
            }
            else
            {
                await SendQuestionForApplicationAsync(application);
            }
        }
        else if (application.Status == ApplicationStatus.New)
        {
            await SendMsgForFinishedApplicationAsync(application, message);
        }
    }
    
    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ошибка при обработке обновления");
        return Task.CompletedTask;
    }

    private async Task ApplicationTeamTitleAnswered(ProjectApplication application, string message)
    {
        var scope = _services.CreateScope();
        var applicationService = scope.ServiceProvider.GetRequiredService<BaseService<ProjectApplication>>();
        application.TeamTitle = message;
        await applicationService.UpdateAsync(application);
        await _botClient.SendMessage(application.ChatId, $"Название команды записано: {message}");
    }
    
    private async Task OnConfirmProjectUpdateReceivedAsync(Update update, Guid caseId)
    {
        var chatId = update.CallbackQuery!.Message!.Chat.Id;
        await _botClient.EditMessageReplyMarkup(chatId, update.CallbackQuery.Message!.Id, replyMarkup: null);
        var selectedCase = await TryGetProjectCaseByIdAsync(caseId);
        if (selectedCase == null)
        {
            await _botClient.EditMessageText(chatId,
                update.CallbackQuery.Message!.Id, "Извините, данный проект больше недоступен.", replyMarkup: null);
            await SendSelectProjectCaseMenu(chatId);
            return;
        }
        await _botClient.SendMessage(chatId, $"Выбран проект: {selectedCase.Title}");
        
        await CreateNewApplicationAsync(caseId, chatId, update.CallbackQuery.From.Username ?? update.CallbackQuery.From.Id.ToString());
        await _botClient.SendMessage(chatId, $"Введите название вашей команды:");
    }

    private async Task ApplicationQuestionAnswered(ProjectApplication application, Message message)
    {
        var questions = await TryGetQuestionForApplicationAsync(application);
        if (questions.CurrentQuestion == null)
        {
            return;
        }
        var scope = _services.CreateScope();
        var applicationService = scope.ServiceProvider.GetRequiredService<BaseService<ProjectApplication>>();
        var answerService = scope.ServiceProvider.GetRequiredService<BaseService<ApplicationQuestionAnswer>>();
        var answer = new ApplicationQuestionAnswer
        {
            Id = Guid.NewGuid(),
            ApplicationId = application.Id,
            QuestionTitle = questions.CurrentQuestion.Title,
            Answer = message.Text!,
            TimeStamp = message.Date.ConvertToTimestamp()
        };
        await answerService.CreateAsync(answer);
        application.CurrentQuestionId = questions.NextQuestion?.Id;
        if (application.CurrentQuestionId == null)
        {
            application.Status = ApplicationStatus.New;
        }
        await applicationService.UpdateAsync(application);
    }
    
    private async Task OnSelectProjectUpdateReceivedAsync(Update update, Guid caseId)
    {
        var projectCase = await TryGetProjectCaseByIdAsync(caseId);
        if (projectCase == null)
        {
            await _botClient.EditMessageText(update.CallbackQuery!.Message!.Chat.Id,
                update.CallbackQuery.Message!.Id, "Извините, данный проект больше недоступен.", replyMarkup: null);
            await SendSelectProjectCaseMenu(update.CallbackQuery.Message!.Chat.Id);
            return;
        }
        await _botClient.EditMessageText(update.CallbackQuery!.Message!.Chat.Id,
            update.CallbackQuery.Message!.Id, GetProjectInfoText(projectCase), ParseMode.Html, replyMarkup: new InlineKeyboardMarkup
            {
                InlineKeyboard = [[new InlineKeyboardButton
                    {
                        Text = "\u2b05\ufe0f Назад",
                        CallbackData = "BackToProjectList"
                    }, new InlineKeyboardButton
                    {
                        Text = "\u2705 Подтвердить выбор",
                        CallbackData = $"ConfirmSelectProjectCase={caseId}"
                    },
                ]]
            });
    }
    
    private string GetProjectInfoText(ProjectCase projectCase)
    {
        return "<b><u>Информация о проекте</u></b>\n\n" +
               $"<b>Название:</b> {projectCase.Title}\n\n" +
               $"<b>Описание:</b> {projectCase.Description}\n\n" +
               $"<b>Цель:</b> {projectCase.Goal}\n\n" +
               $"<b>Критерии:</b> {projectCase.Criteria}\n\n" +
               $"<b>Требуемый результат:</b> {projectCase.RequestedResult}\n\n" +
               $"<b>Свободных мест:</b> {projectCase.MaxTeams - projectCase.AcceptedTeams}";
    }

    private async Task SendSelectProjectCaseMenu(long chatId)
    {
        var availableProjectCases = await GetAvailableProjectCasesAsync();
        if (availableProjectCases.Length == 0)
        {
            await _botClient.SendMessage(chatId, "На данный момент нет ни одного проекта, на который ведётся запись.");
            return;
        }
        await _botClient.SendMessage(chatId, "Выберите один из предложенных кейсов", replyMarkup: new InlineKeyboardMarkup
        {
            InlineKeyboard = availableProjectCases.Select(c => new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton(){ 
                    Text = c.Title + $" (мест: {c.MaxTeams - c.AcceptedTeams})",
                    CallbackData = $"SelectProjectCase={c.Id}"
                }
            })
        });
    }
    
    private async Task SendQuestionForApplicationAsync(ProjectApplication application)
    {
        var questions = await TryGetQuestionForApplicationAsync(application);
        if (questions.CurrentQuestion == null)
        {
            var scope = _services.CreateScope();
            var applicationService = scope.ServiceProvider.GetRequiredService<BaseService<ProjectApplication>>();
            application.Status = ApplicationStatus.New;
            await applicationService.UpdateAsync(application);
            await SendThanksFinishedApplicationAsync(application);
            return;
        }

        await _botClient.SendMessage(application.ChatId, questions.CurrentQuestion.MsgText);
    }
    
    private async Task SendThanksFinishedApplicationAsync(ProjectApplication application)
    {
        await _botClient.SendMessage(application.ChatId,
            "Спасибо! Ваша заявка принята в обработку. Все дальнейшие сообщения, направленные боту, будут переданы кураторам. Сообщения от кураторов также будут направлены вам через данного бота.");
    }
    
    private async Task SendMsgForFinishedApplicationAsync(ProjectApplication application, Message message)
    {
        await CreateNewMessageFromStudentsAsync(application, message);
        await _botClient.SendMessage(application.ChatId, "Сообщение по вашей заявке зарегистрировано в системе и направлено кураторам.",
            replyParameters: new ReplyParameters
            {
                MessageId = message.Id,
                ChatId = message.Chat.Id
            });
    }

    private string GetTextForExistingApplication(ProjectApplication application)
    {
        const string text = "У вас уже есть заявка на проект. ";
        return application.Status switch
        {
            ApplicationStatus.InProgress => text + "Введите ответ на вопрос выше для продолжения регистрации заявки.",
            ApplicationStatus.New => text + "Ожидайте ответ от куратора.",
            ApplicationStatus.Accepted => text + "Ваша заявка принята, ожидайте инструкции от куратора.",
            ApplicationStatus.MeetPlanned => text + "Ваша заявка на рассмотрении, ожидайте инструкции от куратора.",
            ApplicationStatus.Rejected => text + "К сожалению, ваша заявка отклонена.",
            _ => text
        };
    }
}