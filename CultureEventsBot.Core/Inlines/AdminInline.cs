using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Inlines
{
	public class	AdminInline : Inline
	{
		public override string	Name => "new";

		public override async Task	ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var	chatId = callbackQuery.Message.Chat.Id;
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);


			await client.SendTextMessageAsync(
				chatId: chatId,
				text: $"{LanguageHandler.ChooseLanguage(user.Language, "Start writing, then send the message", "Начните печатать, а затем отправьте сообщение")}"
			);
			user.IsAdminWritingPost = true;
			await context.SaveChangesAsync();
			await client.AnswerCallbackQueryAsync(
				callbackQuery.Id,
				$"{LanguageHandler.ChooseLanguage(user.Language, "Start writing", "Начните печатать")}"
			);
		}
	}
}