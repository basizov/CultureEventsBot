using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.API.Core;
using CultureEventsBot.API.Services;
using CultureEventsBot.Core.Commands;
using CultureEventsBot.Core.Core;
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
    public class UpdateController : Controller
    {
		private readonly DataContext _context;
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
        public async Task<OkResult> Post([FromBody]Update update)
        {
            if (update == null)
				return Ok();
            var client = await Bot.GetBotClientAsync(_botService.Configuration);
            var commands = Bot.Commands;
            var message = update.Message;

			var	handler = update.Type switch
			{
				UpdateType.Message => OnMessageHandlerAsync(update.Message, client, commands),
                UpdateType.EditedMessage => OnMessageHandlerAsync(update.Message, client, commands),
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

        private async Task OnMessageHandlerAsync(Message message, TelegramBotClient client, IReadOnlyList<Command> commands)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);

			if (message.Text.StartsWith("/"))
			{
				foreach (var command in commands)
				{
					if (command.Contains(message))
					{
						await command.Execute(message, client, _context);
						break;
					}
				}
			}
			else if (message.Text == "Show event" || message.Text == "Следущее событие")
				await HttpExecute.ShowEventsAsync(_httpClient, message, client, _context, commands, 1);
			else if (message.Text == "Show events 5" || message.Text == "Ближайшие 5 событий")
				await HttpExecute.ShowEventsAsync(_httpClient, message, client, _context, commands, 5);
			else if (message.Text == "Show film" || message.Text == "Следущий фильм")
				await HttpExecute.ShowFilmsAsync(_httpClient, message, client, _context, commands, 1);
			else if (message.Text == "Show films 5" || message.Text == "Ближайшие 5 фильмов")
				await HttpExecute.ShowFilmsAsync(_httpClient, message, client, _context, commands, 5);
			else if (message.Text == "Favourites" || message.Text == "Избранное")
				await HttpExecute.FavouritesAsync(message, client, _context, commands);
			else if (message.Text == "Weather" || message.Text == "Погода")
				await HttpExecute.WeatherAsync(message, client, _httpClient, _context);
			else if (message.Text == "Menu" || message.Text == "Меню")
				await Send.SendMessageAsync(message.Chat.Id, $@"{LanguageHandler.ChooseLanguage(user.Language, "Choose a menu point", "Выберите пункт меню")}:
1. /info
2. /language
3. /rule
4. /keyboard", client);
			else if (message.Text == "Search by categories" || message.Text == "Искать по категориям")
				await HttpExecute.CategoriesAsync(message, client, _context);
			else if (message.Text == "Search by genres" || message.Text == "Искать по жанрам")
				await HttpExecute.GenresAsync(message, client, _context);
			else
				await HttpExecute.AdminAsync(message, client, _context);
		}
        private async Task OnCallbackHandlerAsync(CallbackQuery callbackQuery, TelegramBotClient client)
        {
			if (callbackQuery.Data == "fav" || callbackQuery.Data == "rem")
				await InlineHandlers.EventsAsync(callbackQuery, client, _context);
			else if (callbackQuery.Data == "new")
				await InlineHandlers.AdminAsync(callbackQuery, client, _context);
			else if (callbackQuery.Data == "en" || callbackQuery.Data == "ru")
				await InlineHandlers.LanguageAsync(callbackQuery, client, _context);
			else
				await InlineHandlers.FilterAsync(callbackQuery, client, _context);
        }
        private async Task UnknownTypeHandlerAsync(Update update)
        {
            _logger.LogError($"Unknown update type: {update.Type}");
			await Task.CompletedTask;
        }
    }
}