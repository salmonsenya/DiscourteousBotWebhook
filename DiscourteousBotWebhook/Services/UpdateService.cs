using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DiscourteousBotWebhook.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<UpdateService> _logger;

        public UpdateService(IBotService botService, ILogger<UpdateService> logger)
        {
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public async Task ReplyAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
                return;

            var message = update.Message;
            if (message.Type == MessageType.Text)
            {
                var input = message.Text;
                if (input != null)
                {
                    var regexYes = new Regex(@"(^((да)|(Да)|(ДА))(\?*|\.*|!*)$)|(((\s|,)да\?+)$)");
                    if (regexYes.IsMatch(input))
                    {
                        await _botService.Client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            replyToMessageId: message.MessageId,
                            text: $"пизда");
                    }

                    var regexNo = new Regex(@"(^((нет)|(Нет)|(НЕТ))(\.*|!*)$)");
                    if (regexNo.IsMatch(input))
                    {
                        await _botService.Client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            replyToMessageId: message.MessageId,
                            text: $"пидора ответ");
                    }
                }
            }
        }
    }
}
