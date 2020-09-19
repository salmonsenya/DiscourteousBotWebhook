using System;
using System.Threading.Tasks;
using DiscourteousBotWebhook.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace DiscourteousBotWebhook.Controllers
{
    [Route("bot")]
    public class UpdateController : Controller
    {
        private readonly IUpdateService _updateService;

        public UpdateController(IUpdateService updateService)
        {
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
        }

        //POST
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] object body)
        {
            var update = JsonConvert.DeserializeObject<Telegram.Bot.Types.Update>(body.ToString());
            if (update == null) return Ok();
            await _updateService.ReplyAsync(update);
            return Ok();
        }
    }
}
