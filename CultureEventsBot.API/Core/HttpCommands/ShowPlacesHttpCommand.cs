using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Entities;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.API.Core.HttpCommands
{
	public class ShowPlacesHttpCommand : HttpCommand
	{
		public override string Name => "Next,Далее,Previous,Назад";

		public override bool	Contains(Message message, DataContext context = null)
		{
			var	res = message != null && message.Text != null;
			var	splitName = Name.Split(",");
			Domain.Entities.User user = null;

			if (res)
			{
				foreach (var name in splitName)
				{
					if (context != null)
						user = context.Users.FirstOrDefault(u => u.ChatId == message.Chat.Id);
					res = message.Text.Contains(name) && (user == null || user.ChoosePlan == EChoosePlan.Place);
					if (res) break ;
				}
			}
			return (res);
		}
		public override async Task ExecuteAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, int pageSize = 1)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var	categories = user.Categories == null ? "" : $"&categories={JoinCategories(user.Categories)}";

			user.CurrentPlace += (message.Text.Contains("Next") || message.Text.Contains("Далее") ? 1 : -1);
			if (user.CurrentPlace < 0)
				user.CurrentPlace = 0;
			var page = user.CurrentPlace + 1;
			var placesIds = await HttpWork<Parent>.SendRequestAsync(HttpMethod.Get, $" https://kudago.com/public-api/v1.4/places/?lang={ConvertStringToEnum(user.Language)}&location=kzn&page_size={pageSize}&page={page}{categories}", httpClient);
			if (placesIds != null)
			{
				foreach (var id in placesIds.Results)
				{
					var place = await GetPlaceByIdAsync(id.Id, httpClient, user);

					if (place == null)
						break ;
					await Send.SendPhotoAsync(message.Chat.Id, place.Images.First().Image, $@"
<i>{place.Id}</i>
<b>{place.Title}</b>
{place.Description}
<i>{place.Site_Url}</i>", client, replyMarkup: new InlineKeyboardMarkup(
						InlineKeyboard.GetInlineMatrix(1,
							InlineKeyboardButton.WithCallbackData(LanguageHandler.ChooseLanguage(user.Language, "Add to favourites", "Добавить в избранное"), "fav")
						)
					), parseMode: ParseMode.Html);
					if (context.Places.FirstOrDefault(e => e.Id == place.Id) == null)
						context.Places.Add(place);
					if (pageSize > 1)
						++user.CurrentPlace;
				}
				if (pageSize > 1)
					--user.CurrentPlace;
			}
			context.SaveChanges();
		}

		private async Task<Place>	GetPlaceByIdAsync(int id, IHttpClientFactory httpClient, Domain.Entities.User user)
		{
			var res = await HttpWork<Place>.SendRequestAsync(HttpMethod.Get, $" https://kudago.com/public-api/v1.4/places/{id}", httpClient);

			if (res != null)
			{
				res.Description = res.Description ?? "";
				res.Description = res.Description.Replace("<p>", "");
				res.Description = res.Description.Replace("</p>", "");
			}
			return (res);
		}
	}
}