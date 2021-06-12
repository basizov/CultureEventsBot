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
	public class StartCommand : Command
	{
		public override string Name => @"/start";

		public override int MessageId { get; set; }

		public override async Task Execute(Message message, TelegramBotClient client, DataContext context)
		{
			MessageId = message.MessageId;
			var chatId = message.Chat.Id;
			var	userMessage = message.From;
			var	users = context.Users;
			var user = await users.FirstOrDefaultAsync(u => u.ChatId == chatId);

			if (user == null)
			{
				user = new Domain.Entities.User
				{
					FirstName = userMessage.FirstName,
					SecondName = userMessage.LastName,
					UserName = userMessage.Username,
					ChatId = chatId,
					IsAdmin = false,
					Status = EStatus.User
				};
				
				users.Add(user);
				await context.SaveChangesAsync(); // TODO: Check error: > 0
			}
			// await client.SendTextMessageAsync(chatId, $"Hi, {userMessage.FirstName}, how are you? :)");
			await Send.SendInlineKeyboard(chatId, @"/language", client);
		}

		public override bool Contains(Message message, string inline = null)
		{
			if (message == null || message.Type != MessageType.Text)
				return false;

			return message.Text.Contains(this.Name) || MessageId == message.MessageId;
		}

		public override Task Inline(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context) =>
			throw new System.NotImplementedException();
	}
}