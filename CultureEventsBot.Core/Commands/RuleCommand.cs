using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Commands
{
	public class	RuleCommand : Command
	{
		public override string	Name => "/rule";

		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			
			await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "At the moment, our bot has no restrictions on the terms of use.", "На данный момент наш бот не имеет ограничений в правилах использования."),client);
		}
	}
}