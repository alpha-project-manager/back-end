using Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Services.TelegramBot;

public partial class TelegramBotBackgroundService
{
    private const string ResetCancelArg = "cancel";
    
    private const string ResetConfirmArg = "confirm";
    
    private async Task HandleResetCommandReceived(ProjectApplication application, Message message)
    {
        await _botClient.SendMessage(message.Chat.Id, $"Ваша заявка находится в статусе: {application.Status}\nВы уверены, что хотите удалить заявку на проект?", replyMarkup: new InlineKeyboardMarkup
        {
            InlineKeyboard = [[new InlineKeyboardButton
                {
                    Text = "\u2716\ufe0f Отмена",
                    CallbackData = $"Reset={ResetCancelArg}"
                }, new InlineKeyboardButton
                {
                    Text = "\ud83d\uded1 Удалить заявку",
                    CallbackData = $"Reset={ResetConfirmArg}"
                },
            ]]
        });
    }

    private async Task HandleResetCallbackReceived(Update update, string callbackQueryData)
    {
        var chatId = update.CallbackQuery!.Message!.Chat.Id;
        var arg = callbackQueryData.Split('=')[1];
        if (arg == ResetCancelArg)
        {
            await _botClient.DeleteMessage(chatId, update.CallbackQuery.Message.Id);
            return;
        }
        
        if (await DeleteApplicationAsync(chatId))
        {
            await _botClient.EditMessageText(chatId, update.CallbackQuery.Message.Id, "Ваша заявка была успешно удалена.", replyMarkup: null);
        }
        else
        {
            await _botClient.EditMessageText(chatId, update.CallbackQuery.Message.Id, "Ваша заявка не найдена. Возможно, она уже удалена.", replyMarkup: null);
        }
    }
}