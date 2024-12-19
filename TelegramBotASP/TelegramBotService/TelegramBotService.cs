using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotASP.TelegramBotOptions;

namespace TelegramBotASP.TelegramBotService;

public class TelegramBotService : BackgroundService
{
    //private token
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotService> _logger;
    private readonly TelegramOptions _telegramOptions;
    public TelegramBotService(
        ITelegramBotClient botClient,
        ILogger<TelegramBotService> logger,
        IOptions<TelegramOptions> telegramOptions)
    {
        _botClient = botClient;
        _logger = logger;
        _telegramOptions = telegramOptions.Value;
    }

    public IOptions<TelegramOptions> TelegramOptions { get; }
    public ITelegramBotClient BotClient { get; }
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var botClient = new TelegramBotClient(_telegramOptions.Token);

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = [] // receive all update (типы Сообщений на прием UpdateType.Message)
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            await botClient.ReceiveAsync(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                stoppingToken);

        }

    }

    async Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
            $" Telegram Error Api : [{apiRequestException.ErrorCode}]\n" +
            $"{apiRequestException.Message}",
            _ => exception.ToString()
        };
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;

        switch(messageText)
        {
            case "/start":
                    
        }


        InlineKeyboardMarkup inlineKeyboardMarkup1 = new(new[] {
            InlineKeyboardButton.WithCallbackData("Start","start-bot"),
            InlineKeyboardButton.WithCallbackData("Привет","data1"),
        });
        InlineKeyboardMarkup inlineKeyboardMarkup2 = new([
            [InlineKeyboardButton.WithCallbackData("Записаться", "create-data")],
            [InlineKeyboardButton.WithCallbackData("Перенести", "update-data")],
            [InlineKeyboardButton.WithCallbackData("Отменить", "delete-data")]

            ]);
        



        var chatId = message.Chat.Id;

        Console.WriteLine($"{DateTime.UtcNow}  :  {message.Chat.FirstName ?? "Аноним"}  => |   {message.Text} ");

        Message startMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            //text: "You said: " + message.Text,          // text
            text: "Выбери Опцию",
            //replyMarkup: inlineKeyboardMarkup1,          // button
            replyMarkup: inlineKeyboardMarkup2,
            cancellationToken: cancellationToken);      // endpoint
        
        Console.WriteLine($"{DateTime.UtcNow}  :  {"BOT"}  => |   {startMessage.Text} ");
    }
    private async Task<Message> SendStartMessage(
        
        Message message, 
        CancellationToken cancellationToken)
    {
        Message startMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Выбери Опцию",
            replyMarkup: inlineKeyboardMarkup2,
            cancellationToken: cancellationToken);      




        var inlineMarkup = new InlineKeyboardMarkup()
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow()
                .AddButton("WithCallbackData", "CallbackData")
                .AddButton(InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot"));
        return await bot.SendMessage(msg.Chat, "Inline buttons:", replyMarkup: inlineMarkup);
    }
}
