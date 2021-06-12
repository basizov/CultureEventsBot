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
		public override int MessageId { get; set; }

		public override bool Contains(Message message, string inline = null)
		{
			if (message == null)
				return false;

			return (message.Text != null && message.Text.Contains(this.Name)) || message.MessageId == message.MessageId;
		}

		public override async Task Execute(Message message, TelegramBotClient client, DataContext context)
		{
			MessageId = message.MessageId;
			var replyKeyboardMarkup = new ReplyKeyboardMarkup(
				new KeyboardButton[][]
				{
					new KeyboardButton[] { "Menu", "Weather" },
					new KeyboardButton[] { "Show events", "Show events 5" },
					new KeyboardButton[] { "Favourites" }
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
			var	users = context.Users;
			var user = await users.Include(u => u.Favourites).FirstOrDefaultAsync(u => u.ChatId == chatId);
			var textId = callbackQuery.Message.Caption.Split("\n")[0];
			var ev = await context.Events.FirstOrDefaultAsync(e => e.Id == int.Parse(textId));

			if (callbackQuery.Data == "fav")
			{
				user.Favourites.Add(ev);
				await context.SaveChangesAsync();
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"Added to favourites"
				);
			}
			else if (callbackQuery.Data == "rem" && user.Favourites.Count > 0)
			{
				user.Favourites.Remove(ev);
				await context.SaveChangesAsync();
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"Removed from favourites"
				);
			}
		}
	}
}