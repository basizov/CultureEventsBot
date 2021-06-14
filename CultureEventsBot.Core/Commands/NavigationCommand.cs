using System.Threading.Tasks;
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

		public override bool	Contains(Message message)
		{
			var	res = message != null && message.Text != null;
			var	splitName = Name.Split(",");

			if (res)
			{
				foreach (var name in splitName)
				{
					res = message.Text.Contains(name);
					if (res) break ;
				}
			}
			return (res);
		}
		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			
			user.ChoosePlan = ConvertStringToEnum(message.Text);
			await context.SaveChangesAsync();
			await Send.SendMessageAsync(message.Chat.Id, message.Text, client, replyMarkup: Keyboard.GetNavKeyboard(user));
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