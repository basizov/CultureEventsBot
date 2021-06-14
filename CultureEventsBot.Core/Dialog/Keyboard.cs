using System.Collections.Generic;
using System.Linq;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Domain.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Dialog
{
    public static class	Keyboard
    {
        public static IEnumerable<IEnumerable<KeyboardButton>>	GetKeyboard(int columns, params string[] keywords)
		{
			var keyboard = new List<IEnumerable<KeyboardButton>>();
			var keyboardButtons = new List<KeyboardButton>();

			foreach (var keyword in keywords)
				if (keyboardButtons.FirstOrDefault(b => b.Text == keyword) == null)
					keyboardButtons.Add(GetKeyboardButton(keyword));
			for (int i = 0; i < keyboardButtons.Count; i += columns)
			{
				var tempKeyboardButtons = new List<KeyboardButton>();

				for (int j = 0; j < columns && i + j < keyboardButtons.Count; ++j)
					tempKeyboardButtons.Add(keyboardButtons[i + j]);
				keyboard.Add(tempKeyboardButtons);
			}
			return (keyboard);
		}
        public static KeyboardButton	GetKeyboardButton(string keyword)
		{
			var res = new KeyboardButton(keyword);

			return (res);
		}
        public static IEnumerable<KeyboardButton>	GetKeyboardLine(params string[] keywords)
		{
			var res = new List<KeyboardButton>();

			foreach (var keyword in keywords)
				if (res.FirstOrDefault(b => b.Text == keyword) == null)
					res.Add(GetKeyboardButton(keyword));
			return (res);
		}
        public static IEnumerable<IEnumerable<KeyboardButton>>	GetKeyboardMatrix(params IEnumerable<KeyboardButton>[] keyboards)
		{
			var res = new List<IEnumerable<KeyboardButton>>();

			foreach (var keyboard in keyboards)
				res.Add(keyboard);
			return (res);
		}
		public static ReplyKeyboardMarkup	GetStartKeyboard(User user)
		{
			return (new ReplyKeyboardMarkup(Keyboard.GetKeyboardMatrix(
				Keyboard.GetKeyboardLine($"{LanguageHandler.ChooseLanguage(user.Language, "Menu", "Меню")} {Stickers.Settings}", $"{LanguageHandler.ChooseLanguage(user.Language, "Weather", "Погода")} {Stickers.Weather}"),
				Keyboard.GetKeyboardLine($"{LanguageHandler.ChooseLanguage(user.Language, "Where to go?", "Куда пойти?")} {Stickers.Think}", $"{LanguageHandler.ChooseLanguage(user.Language, "Favourites", "Избранное")} {Stickers.Favourite}")
				// Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Search events by categories", "Искать события по категориям")),
				// Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Search films by genres", "Искать фильмы по жанрам")),
				// Keyboard.GetKeyboardLine(LanguageHandler.ChooseLanguage(user.Language, "Search places by categories", "Искать места по категориям"))
			), resizeKeyboard: true));
		}
		public static ReplyKeyboardMarkup	GetWhereKeyboard(User user)
		{
			return (new ReplyKeyboardMarkup(Keyboard.GetKeyboardMatrix(
				Keyboard.GetKeyboardLine($"{LanguageHandler.ChooseLanguage(user.Language, "Events", "События")} {Stickers.Event}", $"{LanguageHandler.ChooseLanguage(user.Language, "Places", "Места")} {Stickers.Place}"),
				Keyboard.GetKeyboardLine($"{LanguageHandler.ChooseLanguage(user.Language, "Films", "Фильмы")} {Stickers.Movie}", $"{LanguageHandler.ChooseLanguage(user.Language, "Cancel", "Отмена")} {Stickers.Cancel}")
			), resizeKeyboard: true));
		}
		public static ReplyKeyboardMarkup	GetNavKeyboard(User user)
		{
			return (new ReplyKeyboardMarkup(Keyboard.GetKeyboardMatrix(
				Keyboard.GetKeyboardLine($"{Stickers.Previous} {LanguageHandler.ChooseLanguage(user.Language, "Previous", "Назад")}", $"{LanguageHandler.ChooseLanguage(user.Language, "Date", "Дата")} {Stickers.Date}", $"{LanguageHandler.ChooseLanguage(user.Language, "Next", "Далее")} {Stickers.Next}"),
				Keyboard.GetKeyboardLine($"{LanguageHandler.ChooseLanguage(user.Language, "Filtera", "Фильтры")} {Stickers.Filter}", $"{LanguageHandler.ChooseLanguage(user.Language, "Back", "Вернуться")} {Stickers.Cancel}")
			), resizeKeyboard: true));
		}
    }
}