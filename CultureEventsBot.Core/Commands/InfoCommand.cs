using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Commands
{
	public class	InfoCommand : Command
	{
		public override string	Name => "/info";

		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			
			await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, @"
I am a digital bot created with the support of the Ministry of Culture of the Republic of Tatarstan.
Here you can find information about the most relevant events in the city of Kazan.
			", @"
Я цифровой бот, созданный при поддержке Министерства культуры РТ.
Здесь ты можешь найти информацию о самых актуальных мероприятий города Казани.
			"),client);
		}
	}
}