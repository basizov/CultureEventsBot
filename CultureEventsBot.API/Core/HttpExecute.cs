using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CultureEventsBot.Core.Commands;
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
			var page = eventsDB.Count + 1;
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
<b>Price: {(ev.Is_Free ? "Бесплатно" : ev.Price.ToString())}</b>
{ev.Description}
<i>{ev.Site_Url}</i>",
						replyMarkup: new InlineKeyboardMarkup(new[]
						{
							new []
							{
								InlineKeyboardButton.WithCallbackData("Add to favourites", "fav")
							}
						}),
						parseMode: ParseMode.Html
					);
					
					if (command != null)
						command.MessageId = mes.MessageId;
					context.Events.Add(ev);
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
				await client.SendTextMessageAsync(message.Chat.Id, "Favourites:");
			else
				await client.SendTextMessageAsync(message.Chat.Id, "You don't have favourites yet :(");
			foreach (var ev in userEvents)
			{
				var mes = await client.SendPhotoAsync(message.Chat.Id,
					photo: ev.Images.First().Image,
					caption: $@"
<i>{ev.Id}</i>
<b>{ev.Title}</b>
<b>Price: {(ev.Is_Free ? "Бесплатно" : ev.Price.ToString())}</b>
{ev.Description}
<i>{ev.Site_Url}</i>",
					replyMarkup: new InlineKeyboardMarkup(new[]
					{
						new []
						{
							InlineKeyboardButton.WithCallbackData("Remove from favourites", "rem")
						}
					}),
					parseMode: ParseMode.Html
				);
				if (command != null)
					command.MessageId = mes.MessageId;
			}
		}
        public async static Task	Weather(Message message, TelegramBotClient client, IHttpClientFactory httpClient)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, $"https://weatherapi-com.p.rapidapi.com/forecast.json?q=Kazan&days=1");

			request.Headers.Add("x-rapidapi-key", "afa9b0e4c3msh324f1563390aa2dp1bdf92jsn04bf886907db");
			request.Headers.Add("x-rapidapi-host", "weatherapi-com.p.rapidapi.com");
			var clientHttp = httpClient.CreateClient();
			var response = await clientHttp.SendAsync(request);
			var weather = await response.Content.ReadFromJsonAsync<Weather>();

			await client.SendTextMessageAsync(message.Chat.Id, $@"Weather for {weather.Current.Last_Updated}:
temperature is {weather.Current.Temp_C}
feels like {weather.Current.Feelslike_C}
wind speed is {weather.Current.Wind_Kph}
cloud is {weather.Current.Cloud}
");
		}

		private static string ConvertStringToEnum(ELanguage value)
		{
			if (value == ELanguage.English)
				return ("en");
			return ("ru");
		}
    }
}