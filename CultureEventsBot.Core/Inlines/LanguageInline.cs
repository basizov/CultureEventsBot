using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Inlines
{
	public class	LanguageInline : Inline
	{
		public override string Name => "ru,en";

		public override async Task ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var chatId = callbackQuery.Message.Chat.Id;
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

			user.Language = ConvertStringToEnum(callbackQuery.Data);
			context.Users.Update(user);
			await context.SaveChangesAsync();
			
            await client.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"{LanguageHandler.ChooseLanguage(user.Language, "Choosen", "Выбран")} {callbackQuery.Data} {LanguageHandler.ChooseLanguage(user.Language, "language", "язык")}"
            );
			await Send.SendMessageAsync(callbackQuery.Message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Language is changed", "Язык изменен"), client,
				replyMarkup: new ReplyKeyboardMarkup(Keyboard.GetKeyboardMatrix(
					Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Menu", "Меню"), LanguageHandler.ChooseLanguage(user.Language, "Weather", "Погода")),
					Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Show event", "Следущее событие"), LanguageHandler.ChooseLanguage(user.Language, "Show events 5", "Ближайшие 5 событий")),
					Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Show film", "Следущий фильм"), LanguageHandler.ChooseLanguage(user.Language, "Show films 5", "Ближайшие 5 фильмов")),
					Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Show place", "Следуйщее место"), LanguageHandler.ChooseLanguage(user.Language, "Show places 5", "Ближайшие 5 мест")),
					Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Favourites", "Избранное")),
					Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Search events by categories", "Искать события по категориям")),
					Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Search films by genres", "Искать фильмы по жанрам")),
					Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Search places by categories", "Искать места по категориям"))
				))
			);
		}
		public override bool Contains(CallbackQuery callbackQuery)
		{
			var	res = callbackQuery != null && callbackQuery.Data != null;
			var	splitName = Name.Split(",");

			if (res)
			{
				foreach (var name in splitName)
				{
					res = callbackQuery.Data.Contains(name);
					if (res) break ;
				}
			}
			return (res);
		}

		private static ELanguage ConvertStringToEnum(string value)
		{
			var	res = ELanguage.Russian;
			
			if (value == "en")
				res = ELanguage.English;
			return (res);
		}
	}
}