using System.Threading.Tasks;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Core
{
	public static class Send
    {
		public static async Task SendInlineKeyboard(long chatId, string message, TelegramBotClient client)
		{
			// await client.SendChatActionAsync(chatId, ChatAction.Typing);
			// await Task.Delay(500);
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

		public static async Task SendKeyboard(Message message, TelegramBotClient client, DataContext context)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			
			var replyKeyboardMarkup = new ReplyKeyboardMarkup(
				new KeyboardButton[][]
				{
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Menu", "Меню")}", $"{LanguageHandler.ChooseLanguage(user.Language, "Weather", "Погода")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Show event", "Следущее событие")}", $"{LanguageHandler.ChooseLanguage(user.Language, "Show events 5", "Ближайшие 5 событий")}" },
					new KeyboardButton[] { $"{LanguageHandler.ChooseLanguage(user.Language, "Favourites", "Избранное")}" }
				},
				resizeKeyboard: true
			);

			await client.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: "/events",
				replyMarkup: replyKeyboardMarkup
			);
		}
    }
}