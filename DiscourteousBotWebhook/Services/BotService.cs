using Microsoft.Extensions.Options;
using System;
using Telegram.Bot;

namespace DiscourteousBotWebhook.Services
{
    public class BotService : IBotService
    {
        public readonly BotConfiguration _botConfiguration;

        public BotService(IOptions<BotConfiguration> botConfiguration)
        {
            _botConfiguration = botConfiguration?.Value ?? throw new ArgumentNullException(nameof(botConfiguration));
            Client = new TelegramBotClient(_botConfiguration.BotToken);
        }

        public TelegramBotClient Client { get; }
    }
}
