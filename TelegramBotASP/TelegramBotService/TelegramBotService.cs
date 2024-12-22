using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.Xml;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotASP.TelegramBotOptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    async Task HandleErrorAsync(
                            ITelegramBotClient botClient, 
                            Exception exception, 
                            CancellationToken token)
    {
        //var errorMessage = exception switch
        //{
        //    ApiRequestException apiRequestException =>
        //    //$" Telegram Error Api : [{apiRequestException.ErrorCode}]\n" +
        //    //$"{apiRequestException.Message}",
        //    string.Format(
        //        " Telegram Error Api : [{0}]\n" + "{1}",
        //        apiRequestException.ErrorCode,
        //        apiRequestException.Message),
        //    _ => exception.ToString()
        //};
        switch (exception)
        {
            case ApiRequestException apiRequestException:
                _logger.LogError(
                    apiRequestException,
                    "Telegram Error Api: [{ErrorCode}]\n {Message}",
                    apiRequestException.ErrorCode,
                    apiRequestException.Message
                    );
                break;

            default:
                _logger.LogError(exception, "Error while processing message in telegram Bot");
                break;
                //return Task.CompletedTask;
        }
    }

    private async Task MessageTextHandler(Message message, CancellationToken cancellationToken)
    {
        if(message.Text is not { } messageText)
            return;

        switch (messageText.Split(' ')[0])
        {
            case "Start":
                await SendStartMessage(message.Chat.Id, cancellationToken);
                break;
            case "info-message":
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Вот информация!",
                    cancellationToken: cancellationToken);
                break; 
            
        }
    }

    private async Task CallbackQueryHandler(CallbackQuery queryMessage, CancellationToken cancellationToken)
    {
        if(queryMessage.Message is not { } message ) return;    
        switch (queryMessage.Data)
        {
            case "/start":
                await SendStartMessage(message.Id, cancellationToken);
                return;

            case "info-message":
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text   : "Вот информация!",
                    cancellationToken: cancellationToken); 
                return;
        }
    }


    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message: { } message } => MessageTextHandler(message, cancellationToken),
            { CallbackQuery: { } queryMessage } => CallbackQueryHandler(queryMessage, cancellationToken),
            _ => UnknownUpdateDetails(update,cancellationToken)
        };
        await handler;

        InlineKeyboardMarkup inlineKeyboardMarkup1 = new(new[] {
            InlineKeyboardButton.WithCallbackData("Start","start-bot"),
            InlineKeyboardButton.WithCallbackData("Привет","data1"),
        });
        InlineKeyboardMarkup inlineKeyboardMarkup2 = new([
            [InlineKeyboardButton.WithCallbackData("Записаться", "create-data")],
            [InlineKeyboardButton.WithCallbackData("Перенести", "update-data")],
            [InlineKeyboardButton.WithCallbackData("Отменить", "delete-data")]

            ]);
        



        

        //Console.WriteLine($"{DateTime.UtcNow}  :  {message.Chat.FirstName ?? "Аноним"}  => |   {message.Text} ");

        //Message startMessage = await botClient.SendTextMessageAsync(
        //    chatId: message.Chat.Id,
        //    //text: "You said: " + message.Text,          // text
        //    text: "Выбери Опцию",
        //    //replyMarkup: inlineKeyboardMarkup1,          // button
        //    replyMarkup: inlineKeyboardMarkup2,
        //    cancellationToken: cancellationToken);      // endpoint
        
        //Console.WriteLine($"{DateTime.UtcNow}  :  {"BOT"}  => |   {startMessage.Text} ");
    }

    private Task UnknownUpdateDetails(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown type message");
        return Task.CompletedTask;
    }

    private async Task<Message> SendStartMessage(
        long chatId, 
        CancellationToken cancellationToken)
    {
        //1
        var inlineMarkup = new InlineKeyboardMarkup()
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow()
                .AddButton("WithCallbackData", "CallbackData")
                .AddButton(InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot"));
        
        //2
        Message startMessage = await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Выбери Опцию",
            replyMarkup: inlineMarkup,
            cancellationToken: cancellationToken);      




        return await _botClient.SendMessage(startMessage.Chat, "Inline buttons:", replyMarkup: inlineMarkup);
    }
}
