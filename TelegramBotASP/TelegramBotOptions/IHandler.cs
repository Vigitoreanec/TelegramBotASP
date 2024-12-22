using Telegram.Bot.Types;

namespace TelegramBotASP.TelegramBotOptions;

public interface IHandler
{
    Task Handle(TelegramRequest telegramRequest,CancellationToken cancellationToken);
}