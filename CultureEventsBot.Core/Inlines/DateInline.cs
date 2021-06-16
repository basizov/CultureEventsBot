using System;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Inlines
{
	public class	DateInline : Inline
	{
		public override string	Name => "Date";

		public override async Task	ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == callbackQuery.Message.Chat.Id);
			var	date = callbackQuery.Data.Split(" ")[1];
			
			if (user.NewBeginFilterDate == null)
			{
				user.NewBeginFilterDate = DateTime.Parse(date);
				await client.AnswerCallbackQueryAsync(callbackQuery.Id, LanguageHandler.ChooseLanguage(user.Language, "Begin date is applid", "Дата начала установлена"));
			}
			else if (user.NewEndFilterDate == null)
			{
				user.NewEndFilterDate = DateTime.Parse(date);
				await client.AnswerCallbackQueryAsync(callbackQuery.Id, LanguageHandler.ChooseLanguage(user.Language, "End date is applid", "Дата конца установлена"));
				await client.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, $"{LanguageHandler.ChooseLanguage(user.Language, "Date is installed", "Дата уставновлена")} {Stickers.Ok}");
				user.BeginFilterDate = user.NewBeginFilterDate;
				user.EndFilterDate = user.NewEndFilterDate;
				user.FilterDate = DateTime.Now;
				user.NewBeginFilterDate = null;
				user.NewEndFilterDate = null;
			}
			await context.SaveChangesAsync();
		}
	}
}