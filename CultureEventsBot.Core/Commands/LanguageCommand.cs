using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CultureEventsBot.Core.Commands
{
	public class LanguageCommand : Command
	{
		public override string Name => @"/language";

		public override int MessageId { get; set; }

		public override bool Contains(Message message, string inline = null)
		{
			if (message == null || message.Type != MessageType.Text)
				return false;

			return message.Text.Contains(this.Name) || MessageId == message.MessageId;
		}

		public override async Task Execute(Message message, TelegramBotClient client, DataContext context)
		{
			MessageId = message.MessageId;
			await Task.CompletedTask;
		}

		public override async Task Inline(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
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
                $"Received {callbackQuery.Data}"
            );
		}

		private ELanguage ConvertStringToEnum(string value)
		{
			if (value == "en")
				return (ELanguage.English);
			return (ELanguage.Russian);
		}
	}
}