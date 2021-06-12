using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        public async static Task	ShowEvents(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var request = new HttpRequestMessage(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/events/?lang={ConvertStringToEnum(user.Language)}&location=kzn&page_size=1");
			var clientHttp = httpClient.CreateClient();
			var response = await clientHttp.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				var eventsIds = await response.Content.ReadFromJsonAsync<EventParent>();
				var events = new List<Event>();

				foreach (var id in eventsIds.Results)
				{
					request = new HttpRequestMessage(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/events/{id.Id}");
					response = await clientHttp.SendAsync(request);
					var ev = await response.Content.ReadFromJsonAsync<Event>();

					events.Add(ev);
					await client.SendPhotoAsync(message.Chat.Id,
					photo: ev.Images.First().Image,
					caption: $@"
<i>{ev.Id}</i>
<b>{ev.Title}</b>",
					replyMarkup: new InlineKeyboardMarkup(new[]
					{
						new []
						{
							InlineKeyboardButton.WithCallbackData("Add to favourites", "fav")
						}
					}),
					parseMode: ParseMode.Html);
				}
				context.Events.AddRange(events);
				await context.SaveChangesAsync();
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