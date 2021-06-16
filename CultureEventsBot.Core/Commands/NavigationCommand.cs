using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Commands
{
	public class NavigationCommand : Command
	{
		public override string Name => "Events,События,Фильмы,Films,Места,Places";

		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var	mes = $"{LanguageHandler.ChooseLanguage(user.Language, "What are you interested in?", "Что вас интерисует?")} {Stickers.Interests}";
			
			user.ChoosePlan = ConvertStringToEnum(message.Text);
			await context.SaveChangesAsync();
			await Send.SendMessageAsync(message.Chat.Id, mes, client, replyMarkup: Keyboard.GetNavKeyboard(user));
		}

		private EChoosePlan ConvertStringToEnum(string text)
		{
			var	res = EChoosePlan.None;

			if (text.Contains("Events") || text.Contains("События"))
				res = EChoosePlan.Event;
			else if (text.Contains("Films") || text.Contains("Фильмы"))
				res = EChoosePlan.Film;
			else if (text.Contains("Places") || text.Contains("Места"))
				res = EChoosePlan.Place;
			return (res);
		}
	}
}