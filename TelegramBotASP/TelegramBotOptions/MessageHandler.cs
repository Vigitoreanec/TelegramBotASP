using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotASP.TelegramBotOptions;

public class MessageHandler : IHandler
{
    private readonly ITelegramBotClient _botClient;
    private MessageHandler(ITelegramBotClient botClient) => _botClient = botClient;

    public async Task Handle(TelegramRequest telegramRequest, CancellationToken cancellationToken)
    {
        if(telegramRequest.update.Message is null)
            return;

        InlineKeyboardMarkup inlineKeyboardMarkup = new(new[] {
            InlineKeyboardButton.WithCallbackData("Start","start-bot"),
            InlineKeyboardButton.WithCallbackData("Привет","data1"),
        });

        await _botClient.SendTextMessageAsync(
           chatId: telegramRequest.update.Message.Chat.Id,
           text: "Start buttons:",
           replyMarkup: inlineKeyboardMarkup);
    }

    //private async Task Handler(long chatId, CancellationToken cancellationToken)
    //{
    //    InlineKeyboardMarkup inlineKeyboardMarkup = new(new[] {
    //        InlineKeyboardButton.WithCallbackData("Start","start-bot"),
    //        InlineKeyboardButton.WithCallbackData("Привет","data1"),
    //    });

    //    await _botClient.SendMessage(
    //       chatId: chatId,
    //       text:"Start buttons:",
    //       replyMarkup: inlineKeyboardMarkup);
    //}
}
