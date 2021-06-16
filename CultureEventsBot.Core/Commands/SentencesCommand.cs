using System.Linq;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Commands
{
	public class	SentencesCommand : Command
	{
		public override string	Name => "Where to go?,Куда пойти?,Вернуться,Return";

		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var	mes = $"{LanguageHandler.ChooseLanguage(user.Language, "Choose the type of event:", "Выберите тип мероприятия:")} {Stickers.EventType}";
			
			if (message.Text.Contains("Вернуться") || message.Text.Contains("Return"))
				mes = $"{LanguageHandler.ChooseLanguage(user.Language, "Changed your mind?", "Передумали?")} {Stickers.ChangeMind}";
			await Send.SendMessageAsync(message.Chat.Id, mes, client, replyMarkup: Keyboard.GetWhereKeyboard(user));
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