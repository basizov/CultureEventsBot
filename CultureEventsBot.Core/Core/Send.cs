using System.Threading.Tasks;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Core
{
	public static class Send
    {
		public static async Task<Message> SendMessageAsync(long chatId, string message, TelegramBotClient client) =>
			await client.SendTextMessageAsync(chatId: chatId, text: message);
		public static async Task<Message> SendPhotoAsync(long chatId, InputOnlineFile file, string caption, IReplyMarkup replyMarkup, ParseMode parseMode, TelegramBotClient client) =>
			await client.SendPhotoAsync(chatId: chatId, photo: file, caption: caption, replyMarkup: replyMarkup, parseMode: parseMode);
		public static async Task SendInlineKeyboard(long chatId, string message, TelegramBotClient client)
		{
			var inlineKeyboard = new InlineKeyboardMarkup(new[]
			{
				new []
				{
					InlineKeyboardButton.WithCallbackData("Русский", "ru"),
					InlineKeyboardButton.WithCallbackData("English", "en"),
				}
			});

			await client.SendTextMessageAsync(
				chatId: chatId,
				text: message,
				replyMarkup: inlineKeyboard
			);
		}
		public static async Task SendKeyboard(Message message, TelegramBotClient client, DataContext context, string firstMessage = null)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var dialog = firstMessage ?? $"{LanguageHandler.ChooseLanguage(user.Language, "Language is changed", "Язык изменен")}";
			
			var replyKeyboardMarkup = new ReplyKeyboardMarkup(
				new KeyboardButton[][]
				{
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Menu", "Меню")}", $"{LanguageHandler.ChooseLanguage(user.Language, "Weather", "Погода")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Show event", "Следущее событие")}", $"{LanguageHandler.ChooseLanguage(user.Language, "Show events 5", "Ближайшие 5 событий")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Show film", "Следущий фильм")}", $"{LanguageHandler.ChooseLanguage(user.Language, "Show films 5", "Ближайшие 5 фильмов")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Show place", "Следуйщее место")}", $"{LanguageHandler.ChooseLanguage(user.Language, "Show places 5", "Ближайшие 5 мест")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Favourites", "Избранное")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Search events by categories", "Искать события по категориям")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Search films by genres", "Искать фильмы по жанрам")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Search places by categories", "Искать места по категориям")}" }
				},
				resizeKeyboard: true
			);

			await client.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: dialog,
				replyMarkup: replyKeyboardMarkup
			);
		}
    }
}