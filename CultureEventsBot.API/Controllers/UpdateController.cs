using System;
using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.API.Core;
using CultureEventsBot.API.Core.HttpCommands;
using CultureEventsBot.API.Interfaces;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CultureEventsBot.API.Controllers
{
	[ApiController]
    [Route("api/[controller]")]
    public class	UpdateController : Controller
    {
		private readonly DataContext	_context;
    	private readonly IBotService	_botService;
    	private readonly IHttpClientFactory	_httpClient;
    	private readonly ILogger<UpdateController>	_logger;

		public UpdateController(IBotService botService, DataContext context, ILogger<UpdateController> logger, IHttpClientFactory httpClient)
		{
			_context = context;
			_botService = botService;
			_logger = logger;
			_httpClient = httpClient;
		}

        [HttpPost]
        public async Task<OkResult>	Post([FromBody]Update update)
        {
            if (update == null)
				return Ok();
            var	client = await Bot.GetBotClientAsync(_botService.Configuration);
			var	handler = update.Type switch
			{
				UpdateType.Message => OnMessageHandlerAsync(update.Message, client),
                UpdateType.EditedMessage => OnMessageHandlerAsync(update.Message, client),
                UpdateType.CallbackQuery => OnCallbackHandlerAsync(update.CallbackQuery, client),
				_ => UnknownTypeHandlerAsync(update)
			};

            try
            {
                await handler;
            }
            catch (Exception ex)
            {
            	_logger.LogError($"Handle result error: {ex.Message}");
            }
            return Ok();
        }

        private async Task	OnMessageHandlerAsync(Message message, TelegramBotClient client)
		{
			var	user = await _context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);

			foreach (var command in Bot.Commands)
			{
				if (command.Contains(message))
				{
					await command.ExecuteAsync(message, client, _context);
					return ;
				}
			}
			foreach (var command in Bot.HtppCommands)
			{
				if (command.Contains(message))
				{
					await command.ExecuteAsync(_httpClient ,message, client, _context, 1);
					return ;
				}
			}
// 			else if (message.Text == "Search events by categories" || message.Text == "Искать события по категориям")
// 				await HttpExecute.CategoriesAsync(message, client, _context);
// 			else if (message.Text == "Search films by genres" || message.Text == "Искать фильмы по жанрам")
// 				await HttpExecute.GenresAsync(message, client, _context);
// 			else if (message.Text == "Search places by categories" || message.Text == "Искать места по категориям")
// 				await HttpExecute.PlacesAsync(message, client, _context);
// 			else
// 				await HttpExecute.AdminAsync(message, client, _context);
		}
        private async Task OnCallbackHandlerAsync(CallbackQuery callbackQuery, TelegramBotClient client)
        {
			foreach (var inline in Bot.Inlines)
			{
				if (inline.Contains(callbackQuery))
				{
					await inline.ExecuteAsync(callbackQuery, client, _context);
					return ;
				}
			}
			await InlineHandlers.FilterAsync(callbackQuery, client, _context);
        }
        private async Task UnknownTypeHandlerAsync(Update update)
        {
            _logger.LogError($"Unknown update type: {update.Type}");
			await Task.CompletedTask;
        }
    }
}