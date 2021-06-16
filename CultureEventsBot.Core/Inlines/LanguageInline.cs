using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Inlines
{
	public class	LanguageInline : Inline
	{
		public override string Name => "ru,en";

		public override async Task	ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var chatId = callbackQuery.Message.Chat.Id;
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

			user.Language = ConvertStringToEnum(callbackQuery.Data);
			context.Users.Update(user);
			await context.SaveChangesAsync();
			
            await client.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"{LanguageHandler.ChooseLanguage(user.Language, "Choosen", "Выбран")} {ConvertStringToNormal(callbackQuery.Data)} {LanguageHandler.ChooseLanguage(user.Language, "language", "язык")}",
				showAlert: true
            );
			await Send.SendMessageAsync(callbackQuery.Message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Language is changed", "Язык изменен"), client,
				replyMarkup: Keyboard.GetStartKeyboard(user));
		}

		private static ELanguage	ConvertStringToEnum(string value)
		{
			var	res = ELanguage.Russian;
			
			if (value == "en")
				res = ELanguage.English;
			return (res);
		}
		private static string	ConvertStringToNormal(string value)
		{
			var	res = "Русский";
			
			if (value == "en")
				res = "English";
			return (res);
		}
	}
}