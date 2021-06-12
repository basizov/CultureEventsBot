using System.Threading.Tasks;
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
    }
}