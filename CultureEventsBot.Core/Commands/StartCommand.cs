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

		public override async Task Execute(Message message, TelegramBotClient client, DataContext context)
		{
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
					IsAdmin = userMessage.Username == "barkasOff", // TODO: Delete
					Status = EStatus.User
				};
				
				users.Add(user);
				await context.SaveChangesAsync(); // TODO: Check error: > 0
			}
			await Send.SendInlineKeyboard(chatId, LanguageHandler.ChooseLanguage(user.Language, "Choose language:", "Выберите язык:"), client);
			await Send.SendKeyboard(message, client, context);
		}
	}
}