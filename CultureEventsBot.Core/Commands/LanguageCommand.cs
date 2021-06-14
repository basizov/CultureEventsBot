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
	public class	LanguageCommand : Command
	{
		public override string	Name => @"/language";

		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			
			await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Choose language:", "Выберите язык:"), client,
			replyMarkup: new InlineKeyboardMarkup(InlineKeyboard.GetInlineMatrix(
				2,
				InlineKeyboardButton.WithCallbackData("Русский", "ru"),
				InlineKeyboardButton.WithCallbackData("English", "en")
			)));
		}
	}
}