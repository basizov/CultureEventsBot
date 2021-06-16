using System.Linq;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Commands
{
	public class	KeyboardCommand : Command
	{
		public override string	Name => "/keyboard,Отмена,Back";

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
			var	mes = $"{LanguageHandler.ChooseLanguage(user.Language, "Would you like to see the weather?", "Не хотите ли посмотреть погоду?")} {Stickers.SeeWether}";
			
			await Send.SendMessageAsync(message.Chat.Id, message.Text == "/keyboard" ? LanguageHandler.ChooseLanguage(user.Language, "Keyboard is available", "Клавиатура включена") : mes, client, replyMarkup: Keyboard.GetStartKeyboard(user));
			if (context.Categories.Any(c => c.IsChecked))
			{
				var	categories = await context.Categories.Where(c => c.IsChecked).ToListAsync();
				
				foreach (var category in categories)
					category.IsChecked = false;
				await context.SaveChangesAsync();
			}
		}
	}
}