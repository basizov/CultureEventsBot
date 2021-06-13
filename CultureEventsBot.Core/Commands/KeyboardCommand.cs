using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Commands
{
	public class KeyboardCommand : Command
	{
		public override string Name => "/keyboard";

		public override async Task Execute(Message message, TelegramBotClient client, DataContext context)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			
			await Send.SendKeyboard(message, client, context, LanguageHandler.ChooseLanguage(user.Language, "Keyboard is available", "Клавиатура включена"));
		}
	}
}