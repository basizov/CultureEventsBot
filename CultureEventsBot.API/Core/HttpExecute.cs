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
        public async static Task	ShowEvents(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, IReadOnlyList<Command> commands)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var request = new HttpRequestMessage(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/events/?lang={ConvertStringToEnum(user.Language)}&location=kzn&page_size=1");
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

					var mes = await client.SendPhotoAsync(message.Chat.Id,
					photo: ev.Images.First().Image,
					caption: $@"
<i>{ev.Id}</i>
<b>{ev.Title}</b>
<i>{ev.Site_Url}</i>",
					replyMarkup: new InlineKeyboardMarkup(new[]
					{
						new []
						{
							InlineKeyboardButton.WithCallbackData("Add to favourites", "fav")
						}
					}),
					parseMode: ParseMode.Html);
					
					if (command != null)
						command.MessageId = mes.MessageId;
					context.Events.Add(ev);
				}
				context.SaveChanges();
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