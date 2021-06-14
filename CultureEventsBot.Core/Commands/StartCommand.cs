using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Commands
{
	public class	StartCommand : Command
	{
		public override string	Name => "/start";

		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	chatId = message.Chat.Id;
			var	userMessage = message.From;
			var	users = context.Users;
			var	user = await users.FirstOrDefaultAsync(u => u.ChatId == chatId);

			if (user == null)
			{
				user = new Domain.Entities.User
				{
					FirstName = userMessage.FirstName,
					SecondName = userMessage.LastName,
					UserName = userMessage.Username,
					ChatId = chatId,
					IsAdmin = userMessage.Username == "barkasOff", // TODO: Delete
					Status = EStatus.User
				};
				
				users.Add(user);
				await context.SaveChangesAsync(); // TODO: Check error: > 0
				await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, @"
Hi, I am a digital bot created with the support of the Ministry of Culture of the Republic of Tatarstan.
Here you can find information about the most relevant events in the city of Kazan.
			", @"
Привет я цифровой бот, созданный при поддержке Министерства культуры РТ.
Здесь ты можешь найти информацию о самых актуальных мероприятий города Казани.
				"), client,
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
				await Send.SendMessageAsync(chatId, LanguageHandler.ChooseLanguage(user.Language, "Choose language:", "Выберите язык:"), client,
				replyMarkup: new InlineKeyboardMarkup(InlineKeyboard.GetInlineMatrix(
					2,
					InlineKeyboardButton.WithCallbackData("Русский", "ru"),
					InlineKeyboardButton.WithCallbackData("English", "en")
				)));
			}
		}
	}
}