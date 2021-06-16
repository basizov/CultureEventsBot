using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Inlines
{
	public class	DateNavInline : Inline
	{
		public override string	Name => "next-day,next-date,prev-day,prev-date";

		public override async Task	ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == callbackQuery.Message.Chat.Id);
			var	months = new []
			{
				LanguageHandler.ChooseLanguage(user.Language, "January", "Январь"),
				LanguageHandler.ChooseLanguage(user.Language, "February", "Февраль"),
				LanguageHandler.ChooseLanguage(user.Language, "March", "Март"),
				LanguageHandler.ChooseLanguage(user.Language, "April", "Апрель"),
				LanguageHandler.ChooseLanguage(user.Language, "May", "Май"),
				LanguageHandler.ChooseLanguage(user.Language, "June", "Июнь"),
				LanguageHandler.ChooseLanguage(user.Language, "July", "Июль"),
				LanguageHandler.ChooseLanguage(user.Language, "August", "Август"),
				LanguageHandler.ChooseLanguage(user.Language, "Septembery", "Сентябрь"),
				LanguageHandler.ChooseLanguage(user.Language, "October", "Октябрь"),
				LanguageHandler.ChooseLanguage(user.Language, "November", "Ноябрь"),
				LanguageHandler.ChooseLanguage(user.Language, "December", "Декабрь")
			};

			if (callbackQuery.Data.Contains("next-day"))
				user.FilterDate = user.FilterDate.AddMonths(1);
			else if (callbackQuery.Data.Contains("prev-day"))
				user.FilterDate = user.FilterDate.AddMonths(-1);
			else if (callbackQuery.Data.Contains("next-date"))
				user.FilterDate = user.FilterDate.AddYears(1);
			else if (callbackQuery.Data.Contains("prev-date"))
				user.FilterDate = user.FilterDate.AddYears(-1);
			await context.SaveChangesAsync();
			await client.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, callbackQuery.Message.Text, replyMarkup: new InlineKeyboardMarkup(InlineKeyboard.GetDateInlineKeyboard(user)));
			await client.AnswerCallbackQueryAsync(callbackQuery.Id, $"{months[user.FilterDate.Month - 1]} {user.FilterDate.Year}");
		}
	}
}