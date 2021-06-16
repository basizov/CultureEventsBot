using System;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Inlines
{
	public class	DateSureInline : Inline
	{
		public override string	Name => "save-date,cancel-date,clear-date";

		public override async Task	ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == callbackQuery.Message.Chat.Id);
			var	mes = $"{LanguageHandler.ChooseLanguage(user.Language, "Date is installed", "Дата уставновлена")} {Stickers.Ok}";

			if (callbackQuery.Data.Contains("save-date"))
			{
				user.BeginFilterDate = user.NewBeginFilterDate;
				user.EndFilterDate = user.NewEndFilterDate;
			}
			else if (callbackQuery.Data.Contains("cancel-date"))
				mes = $"{LanguageHandler.ChooseLanguage(user.Language, "Date isn't installed", "Дата не уставновлена")} {Stickers.Stop}";
			else if (callbackQuery.Data.Contains("clear-date"))
			{
				mes = $"{LanguageHandler.ChooseLanguage(user.Language, "Date's filter is cleared", "Фильтр даты очищен")} {Stickers.Clear}";
				user.BeginFilterDate = null;
				user.EndFilterDate = null;
			}
			user.FilterDate = DateTime.Now;
			user.NewBeginFilterDate = null;
			user.NewEndFilterDate = null;
			await context.SaveChangesAsync();
			await client.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, mes);
		}
	}
}