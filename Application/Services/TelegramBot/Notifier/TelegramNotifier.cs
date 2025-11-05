using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.Services.TelegramBot.Notifier;

public class TelegramNotifier : ITelegramNotifier
{
    private readonly ITelegramBotClient _bot;
    
    public TelegramNotifier(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task SendMsgFromTutorsAsync(long chatId, string text, CancellationToken ct = default)
    {
        await SendTextAsync(chatId, "*Cообщение от кураторов:*\n\n" + text, ParseMode.Markdown, ct);
    }

    public async Task SendTextAsync(long chatId, string text, ParseMode parseMode = ParseMode.None, CancellationToken ct = default)
    {
        await _bot.SendMessage(chatId, text, cancellationToken: ct, parseMode: parseMode);
    }
}