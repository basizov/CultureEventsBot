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
        public static async Task	AdminAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var chatId = callbackQuery.Message.Chat.Id;
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

			if (callbackQuery.Data == "new")
			{
				await client.SendTextMessageAsync(
					chatId: chatId,
					text: $"{LanguageHandler.ChooseLanguage(user.Language, "Start writing, then send the message", "Начните печатать, а затем отправьте сообщение")}"
				);
				user.IsAdminWritingPost = true;
				await context.SaveChangesAsync();
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Start writing", "Начните печатать")}"
				);
			}
		}
        public static async Task	EventsAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var chatId = callbackQuery.Message.Chat.Id;
			var	users = context.Users;
			var user = await users.Include(u => u.Favourites).FirstOrDefaultAsync(u => u.ChatId == chatId);
			var textId = callbackQuery.Message.Caption.Split("\n")[0];
			Favourite ev = await context.Events.FirstOrDefaultAsync(e => e.Id == int.Parse(textId));

			if (ev == null)
				ev = await context.Films.FirstOrDefaultAsync(e => e.Id == int.Parse(textId));
			if (callbackQuery.Data == "fav" && users.FirstOrDefault(e => e.Favourites.Contains(ev)) == null)
			{
				user.Favourites.Add(ev);
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Added to favourites", "Добавлено в избранное")}"
				);
			}
			else if (callbackQuery.Data == "fav")
			{
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Already in favourites", "Было ранее добавлено в избранное")}"
				);
			}
			else if (callbackQuery.Data == "rem" && user.Favourites.Count > 0)
			{
				user.Favourites.Remove(ev);
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Removed from favourites", "Удалено из избранного")}"
				);
			}
			else if (callbackQuery.Data == "rem")
			{
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Already removed", "Уже удалено")}"
				);
			}
			await context.SaveChangesAsync();
		}
        public static async Task	LanguageAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var chatId = callbackQuery.Message.Chat.Id;
			var	userMessage = callbackQuery.Message.From;
			var	users = context.Users;
			var user = await users.FirstOrDefaultAsync(u => u.ChatId == chatId);

			user.Language = ConvertStringToEnum(callbackQuery.Data);
			users.Update(user);
			await context.SaveChangesAsync(); // TODO: Check error: > 0
            await client.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"{LanguageHandler.ChooseLanguage(user.Language, "Choosen", "Выбран")} {callbackQuery.Data} {LanguageHandler.ChooseLanguage(user.Language, "language", "язык")}"
            );
			await Send.SendKeyboard(callbackQuery.Message, client, context);
		}
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
			var filters = new List<Favourite>();

			foreach (var item in eventsDb)
				if (item.Categories != null && item.Categories.FirstOrDefault(c => c == callbackQuery.Data) != null)
					filters.Add(item);
			foreach (var item in filmsDb)
				if (item.Genres != null && item.Genres.FirstOrDefault(c => c.Name == callbackQuery.Data) != null)
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
                $"{LanguageHandler.ChooseLanguage(user.Language, "Searched elements in the category", "Были найдены элементы в категории ")} {callbackQuery.Data}"
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