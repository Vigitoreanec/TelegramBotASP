using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBotASP.TelegramBotOptions;
using TelegramBotASP.TelegramBotService;

var builder = Host.CreateApplicationBuilder(args);
// Setup bot configuration
//var botConfigSection = builder.Configuration.GetSection("Telegram");
builder.Services.AddHostedService<TelegramBotService>();

builder.Services.AddTransient<ITelegramBotClient, TelegramBotClient>(serviceProvider =>
{
    var token = serviceProvider.GetRequiredService<IOptions<TelegramOptions>>().Value.Token;
    return new(token);
}
);

//builder.Services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
//                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
//                {
//                    BotConfiguration? botConfiguration = sp.GetService<IOptions<BotConfiguration>>()?.Value;
//                    ArgumentNullException.ThrowIfNull(botConfiguration);
//                    TelegramBotClientOptions options = new(botConfiguration.BotToken);
//                    return new TelegramBotClient(options, httpClient);
//                });

builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection(TelegramOptions.Telegram));
var host = builder.Build();
host.Run();