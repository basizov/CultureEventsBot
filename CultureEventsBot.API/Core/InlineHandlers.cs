using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public static class InlineHandlers
    {
        public static async Task	FilterAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == callbackQuery.Message.Chat.Id);
			var eventsDb = await context.Events
				.Include(e => e.Images)
				.ToListAsync();
			var filmsDb = await context.Films
				.Include(e => e.Images)
				.Include(e => e.Genres)
				.ToListAsync();
			var placesDb = await context.Places
				.Include(e => e.Images)
				.ToListAsync();
			var filters = new List<Favourite>();

			foreach (var item in eventsDb)
				if (item.Categories != null && item.Categories.FirstOrDefault(c => c == callbackQuery.Data) != null)
					filters.Add(item);
			foreach (var item in filmsDb)
				if (item.Genres != null && item.Genres.FirstOrDefault(c => c.Name == callbackQuery.Data) != null)
					filters.Add(item);
			foreach (var item in placesDb)
				if (item.Categories != null && item.Categories.FirstOrDefault(c => c == callbackQuery.Data) != null)
					filters.Add(item);
			foreach (var fil in filters)
			{
				var mes = await client.SendPhotoAsync(callbackQuery.Message.Chat.Id,
					photo: fil.Images.First().Image,
					caption: $@"
<i>{fil.Id}</i>
<b>{fil.Title}</b>
{ConvertFavouriteToEvent(user, fil)}
{fil.Description}
<i>{fil.Site_Url}</i>",
					replyMarkup: new InlineKeyboardMarkup(new[]
					{
						new []
						{
							InlineKeyboardButton.WithCallbackData(LanguageHandler.ChooseLanguage(user.Language, "Add to favourites", "Добавить в избранное"), "fav")
						}
					}),
					parseMode: ParseMode.Html
				);
			}
            await client.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"{LanguageHandler.ChooseLanguage(user.Language, "Searched elements in the category", "Были найдены элементы в категории")} {callbackQuery.Data}"
            );
		}

		private static ELanguage ConvertStringToEnum(string value)
		{
			if (value == "en")
				return (ELanguage.English);
			return (ELanguage.Russian);
		}
		private static string	ConvertFavouriteToEvent(Domain.Entities.User user, Favourite favourite)
		{
			var res = "";

			if (favourite is Event ev)
				res = $"<b>{LanguageHandler.ChooseLanguage(user.Language, "Price", "Цена")}: {(ev.Is_Free ? $"{LanguageHandler.ChooseLanguage(user.Language, "Free", "Бесплатно")}" : ev.Price)}</b>";
			return (res);
		}
    }
}