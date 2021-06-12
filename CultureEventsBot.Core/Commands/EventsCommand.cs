using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Commands
{
	public class EventsCommand : Command
	{
		public override string Name => @"/events";

		public override bool Contains(Message message)
		{
			if (message == null || message.Type != MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}

		public override async Task Execute(Message message, TelegramBotClient client, DataContext context)
		{
			var replyKeyboardMarkup = new ReplyKeyboardMarkup(
				new KeyboardButton[][]
				{
					new KeyboardButton[] { "Show events" }
				},
				resizeKeyboard: true
			);

			await client.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: "/events",
				replyMarkup: replyKeyboardMarkup
			);
		}

		public override async Task Inline(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var chatId = callbackQuery.Message.Chat.Id;
			var	userMessage = callbackQuery.Message.From;
			var	users = context.Users;
			var user = await users.FirstOrDefaultAsync(u => u.ChatId == chatId);

			users.Update(user);
			await context.SaveChangesAsync(); // TODO: Check error: > 0
            await client.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"Received {callbackQuery.Data}"
            );
		}
	}
}