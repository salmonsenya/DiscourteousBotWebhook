using Telegram.Bot;

namespace DiscourteousBotWebhook.Services
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
