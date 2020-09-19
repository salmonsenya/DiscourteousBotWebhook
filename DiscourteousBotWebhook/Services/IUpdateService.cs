using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DiscourteousBotWebhook.Services
{
    public interface IUpdateService
    {
        Task ReplyAsync(Update update);
    }
}
