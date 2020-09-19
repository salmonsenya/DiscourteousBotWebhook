using Microsoft.AspNetCore.Mvc;

namespace DiscourteousBotWebhook.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("health")]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
