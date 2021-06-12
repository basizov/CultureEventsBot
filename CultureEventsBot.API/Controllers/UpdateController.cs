using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CultureEventsBot.API.Services;
using CultureEventsBot.Core.Commands;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace CultureEventsBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
		private readonly DataContext _context;
    	private readonly IBotService	_botService;
    	private readonly ILogger<UpdateController>	_logger;

		public UpdateController(IBotService botService, DataContext context, ILogger<UpdateController> logger)
		{
			_context = context;
			_botService = botService;
			_logger = logger;
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
                UpdateType.CallbackQuery => OnCallbackHandlerAsync(update.CallbackQuery, client, commands),
                UpdateType.InlineQuery => OnInlineHandlerAsync(update.InlineQuery, client),
                UpdateType.ChosenInlineResult => OnChosenInlineResultHandlerAsync(update.ChosenInlineResult, client, message.Chat.Id),
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
			if (commands != null)
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
		}

        private async Task OnCallbackHandlerAsync(CallbackQuery callbackQuery, TelegramBotClient client, IReadOnlyList<Command> commands)
        {
			if (commands != null)
			{
				foreach (var command in commands)
				{
					if (command.Contains(callbackQuery.Message))
					{
						await command.Inline(callbackQuery, client, _context);
						break;
					}
				}
			}
        }
		
        private async Task OnInlineHandlerAsync(InlineQuery inlineQuery, TelegramBotClient client)
        {
            _logger.LogDebug($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };

            await client.AnswerInlineQueryAsync(
                inlineQuery.Id,
                results,
                isPersonal: true,
                cacheTime: 0
            );
        }

        private async Task OnChosenInlineResultHandlerAsync(ChosenInlineResult chosenInlineResult, TelegramBotClient client, long chatId)
        {
			await client.SendTextMessageAsync(chatId, "Received inline result: {chosenInlineResult.ResultId}");
			await Task.CompletedTask;
        }

        private async Task UnknownTypeHandlerAsync(Update update)
        {
            _logger.LogError($"Unknown update type: {update.Type}");
			await Task.CompletedTask;
        }
    }
}