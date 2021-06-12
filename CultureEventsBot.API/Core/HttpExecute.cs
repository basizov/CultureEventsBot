using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CultureEventsBot.Core.Commands;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Domain.Entities;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.API.Core
{
	public static class HttpExecute
    {
        public async static Task	ShowEvents(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, IReadOnlyList<Command> commands, int pageSize)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var eventsDB = await context.Events.ToListAsync();
			var page = user.CurrentEvent + 1;
			var request = new HttpRequestMessage(HttpMethod.Get,
				$"https://kudago.com/public-api/v1.4/events/?lang={ConvertStringToEnum(user.Language)}&location=kzn&page_size={pageSize}&page={page}");
			var clientHttp = httpClient.CreateClient();
			var response = await clientHttp.SendAsync(request);
			var command = commands.AsEnumerable().FirstOrDefault(c => c.Name == "/events");

			if (response.IsSuccessStatusCode)
			{
				var eventsIds = await response.Content.ReadFromJsonAsync<EventParent>();

				foreach (var id in eventsIds.Results)
				{
					request = new HttpRequestMessage(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/events/{id.Id}");
					response = await clientHttp.SendAsync(request);
					var ev = await response.Content.ReadFromJsonAsync<Event>();

					ev.Description = ev.Description.Remove(0, 3);
					ev.Description = ev.Description.Remove(ev.Description.Length - 5);
					var mes = await client.SendPhotoAsync(message.Chat.Id,
						photo: ev.Images.First().Image,
						caption: $@"
<i>{ev.Id}</i>
<b>{ev.Title}</b>
<b>{LanguageHandler.ChooseLanguage(user.Language, "Price", "Цена")}: {(ev.Is_Free ? $"{LanguageHandler.ChooseLanguage(user.Language, "Free", "Бесплатно")}" : ev.Price.ToString())}</b>
{ev.Description}
<i>{ev.Site_Url}</i>",
						replyMarkup: new InlineKeyboardMarkup(new[]
						{
							new []
							{
								InlineKeyboardButton.WithCallbackData($"{LanguageHandler.ChooseLanguage(user.Language, "Add to favourites", "Добавить в избранное")}", "fav")
							}
						}),
						parseMode: ParseMode.Html
					);
					
					if (command != null)
						command.MessageId = mes.MessageId;
					if (context.Events.FirstOrDefault(e => e.Id == ev.Id) == null)
						context.Events.Add(ev);
					++user.CurrentEvent;
				}
				context.SaveChanges();
			}
		}
        public async static Task	Favourites(Message message, TelegramBotClient client, DataContext context, IReadOnlyList<Command> commands)
		{
			var user = await context.Users
				.Include(u => u.Favourites)
				.ThenInclude(u => u.Images)
				.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var userEvents = user.Favourites;
			var command = commands.AsEnumerable().FirstOrDefault(c => c.Name == "/events");

			if (userEvents.Count > 0)
				await client.SendTextMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Favourites:", "Избранные:"));
			else
				await client.SendTextMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "You don't have favourites events yet :(", "У вас нет избранных событий:("));
			foreach (var ev in userEvents)
			{
				var mes = await client.SendPhotoAsync(message.Chat.Id,
					photo: ev.Images.First().Image,
					caption: $@"
<i>{ev.Id}</i>
<b>{ev.Title}</b>
<b>{LanguageHandler.ChooseLanguage(user.Language, "Price:", "Цена:")} {(ev.Is_Free ? LanguageHandler.ChooseLanguage(user.Language, "Free", "Бесплатно") : ev.Price.ToString())}</b>
{ev.Description}
<i>{ev.Site_Url}</i>",
					replyMarkup: new InlineKeyboardMarkup(new[]
					{
						new []
						{
							InlineKeyboardButton.WithCallbackData(LanguageHandler.ChooseLanguage(user.Language, "Remove from favourites", "Удалить из избранных"), "rem")
						}
					}),
					parseMode: ParseMode.Html
				);
				if (command != null)
					command.MessageId = mes.MessageId;
			}
		}
        public async static Task	Weather(Message message, TelegramBotClient client, IHttpClientFactory httpClient, DataContext context)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var request = new HttpRequestMessage(HttpMethod.Get, $"https://weatherapi-com.p.rapidapi.com/forecast.json?q=Kazan&days=1");

			request.Headers.Add("x-rapidapi-key", "afa9b0e4c3msh324f1563390aa2dp1bdf92jsn04bf886907db");
			request.Headers.Add("x-rapidapi-host", "weatherapi-com.p.rapidapi.com");
			var clientHttp = httpClient.CreateClient();
			var response = await clientHttp.SendAsync(request);
			var weather = await response.Content.ReadFromJsonAsync<Weather>();

			await client.SendTextMessageAsync(message.Chat.Id, $@"{LanguageHandler.ChooseLanguage(user.Language, "Weather for", "Погода на")} {weather.Current.Last_Updated}:
{LanguageHandler.ChooseLanguage(user.Language, "Temperature is", "Температура")} {weather.Current.Temp_C}
{LanguageHandler.ChooseLanguage(user.Language, "Feels like ", "Ощущается как")} {weather.Current.Feelslike_C}
{LanguageHandler.ChooseLanguage(user.Language, "Wind speed is", "Скорость ветра:")} {weather.Current.Wind_Kph}
{LanguageHandler.ChooseLanguage(user.Language, "Cloud is", "Облачность")} {weather.Current.Cloud}
");
		}
        public async static Task	Admin(Message message, TelegramBotClient client, DataContext context)
		{
			var users = context.Users;
			var	user = await users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);

			if (user.IsAdmin && user.IsAdminWritingPost)
			{
				foreach	(var u in users)
				{
					if (u.MayNotification && !u.IsAdmin)
					{
						await client.ForwardMessageAsync(u.ChatId, message.Chat.Id, message.MessageId);
					}
				}
			}
		}

		private static string ConvertStringToEnum(ELanguage value)
		{
			if (value == ELanguage.English)
				return ("en");
			return ("ru");
		}
    }
}