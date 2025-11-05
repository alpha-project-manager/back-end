using Telegram.Bot.Types.Enums;

namespace Application.Services.TelegramBot.Notifier;

public interface ITelegramNotifier
{
    
    Task SendMsgFromTutorsAsync(long chatId, string text, CancellationToken ct = default);
    
    Task SendTextAsync(long chatId, string text, ParseMode parseMode, CancellationToken ct = default);
}