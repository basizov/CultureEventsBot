using System.Threading.Tasks;
using CultureEventsBot.API.Services;
using CultureEventsBot.Core.Core;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace CultureEventsBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
    	private readonly IBotService _botService;

		public UpdateController(IBotService botService) =>
			_botService = botService;

        [HttpPost]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            if (update == null) return Ok();

            var commands = Bot.Commands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync(_botService.Configuration);

            foreach (var command in commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, botClient);
                    break;
                }
            }
            return Ok();
        }
    }
}