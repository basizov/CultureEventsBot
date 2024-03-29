using System;
using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.API.Core;
using CultureEventsBot.API.Interfaces;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Enums;
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

      if (message.From.Username == "irada1307" && user == null)
      {
				user = new Domain.Entities.User
				{
					FirstName = message.From.FirstName,
					SecondName = message.From.LastName,
					UserName = message.From.Username,
					ChatId = message.Chat.Id,
					IsAdmin = message.From.Username == "barkasOff", // TODO: Delete
					Status = EStatus.User
				};
				
				_context.Users.Add(user);
				await _context.SaveChangesAsync();
      }
      if (message.From.Username == "irada1307")
      {
        var barkasOff = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "barkasOff");

        await client.SendTextMessageAsync(barkasOff.ChatId, "Министр культуры!!");
      }
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
        if (command.Contains(message, _context))
        {
          await command.ExecuteAsync(_httpClient ,message, client, _context, 1);
          return ;
        }
      }
      await Send.SendMessageAsync(message.Chat.Id, $"{LanguageHandler.ChooseLanguage(user.Language, "I don't know how to do that yet", "Я пока так еще не умею")} {Stickers.SadFace}", client);
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
      await client.AnswerCallbackQueryAsync(callbackQuery.Id, callbackQuery.Data);
    }

    private async Task UnknownTypeHandlerAsync(Update update)
    {
      _logger.LogError($"Unknown update type: {update.Type}");
      await Task.CompletedTask;
    }
  }
}