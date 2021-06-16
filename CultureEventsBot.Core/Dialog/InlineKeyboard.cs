using System;
using System.Collections.Generic;
using System.Linq;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Domain.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Dialog
{
	public static class	InlineKeyboard
    {
        public static IEnumerable<IEnumerable<InlineKeyboardButton>>	GetInlineKeyboard(int columns, params string[] keywords)
		{
			var inlineKeyboard = new List<IEnumerable<InlineKeyboardButton>>();
			var inlineKeyboardButtons = new List<InlineKeyboardButton>();

			foreach (var keyword in keywords)
			{
				if (inlineKeyboardButtons.FirstOrDefault(b => b.Text == keyword) == null)
				{
					var btn = new InlineKeyboardButton
					{
						Text = keyword,
						CallbackData = keyword
					};

					inlineKeyboardButtons.Add(btn);
				}
			}
			for (int i = 0; i < inlineKeyboardButtons.Count; i += columns)
			{
				var tempKeyboardButtons = new List<InlineKeyboardButton>();

				for (int j = 0; j < columns && i + j < inlineKeyboardButtons.Count; ++j)
					tempKeyboardButtons.Add(inlineKeyboardButtons[i + j]);
				inlineKeyboard.Add(tempKeyboardButtons);
			}
			return (inlineKeyboard);
		}
        public static IEnumerable<IEnumerable<InlineKeyboardButton>>	GetInlineMatrix(int columns, params InlineKeyboardButton[] inlineKeyboardButtons)
		{
			var inlineKeyboard = new List<IEnumerable<InlineKeyboardButton>>();

			for (int i = 0; i < inlineKeyboardButtons.Length; i += columns)
			{
				var tempKeyboardButtons = new List<InlineKeyboardButton>();

				for (int j = 0; j < columns && i + j < inlineKeyboardButtons.Length; ++j)
					tempKeyboardButtons.Add(inlineKeyboardButtons[i + j]);
				inlineKeyboard.Add(tempKeyboardButtons);
			}
			return (inlineKeyboard);
		}
        public static IEnumerable<InlineKeyboardButton>	GetInlineKeyboardLine(Dictionary<string, string> words)
		{
			var res = new List<InlineKeyboardButton>();

			foreach (var word in words)
				if (res.FirstOrDefault(b => b.Text == word.Key) == null)
					res.Add(GetInlineKeyboardButton(word.Value, word.Key));
			return (res);
		}
        public static InlineKeyboardButton	GetInlineKeyboardButton(string value, string key)
		{
			var res = new InlineKeyboardButton
			{
				Text = value,
				CallbackData = key
			};

			return (res);
		}
        public static IEnumerable<IEnumerable<InlineKeyboardButton>>	GetDateInlineKeyboard(User user)
		{
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
			var	currentDate = user.FilterDate;
			var	beginMonthDay = new DateTime(currentDate.Year, currentDate.Month, 1);
			var	endMonthDay = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
			var inlineKeyboard = new List<IEnumerable<InlineKeyboardButton>>();
			var inlineKeyboardButtons = new List<InlineKeyboardButton>();
			
			inlineKeyboard.Add(InlineKeyboard.GetInlineKeyboardLine(new Dictionary<string, string>
			{
				{ "prev-date", "<<" },
				{ currentDate.Year.ToString(), currentDate.Year.ToString() },
				{ "next-date", ">>" }
			}));
			inlineKeyboard.Add(InlineKeyboard.GetInlineKeyboardLine(new Dictionary<string, string>
			{
				{ "prev-day", "<" },
				{ months[currentDate.Month - 1], months[currentDate.Month - 1] },
				{ "next-day", ">" }
			}));
			inlineKeyboard.Add(InlineKeyboard.GetInlineKeyboardLine(new Dictionary<string, string>
			{
				{ LanguageHandler.ChooseLanguage(user.Language, "Monday", "Понедельник"), LanguageHandler.ChooseLanguage(user.Language, "Mon", "Пон") },
				{ LanguageHandler.ChooseLanguage(user.Language, "Tueday", "Вторник"), LanguageHandler.ChooseLanguage(user.Language, "Tu", "Вт") },
				{ LanguageHandler.ChooseLanguage(user.Language, "Wednesday", "Среда"), LanguageHandler.ChooseLanguage(user.Language, "Wed", "Ср") },
				{ LanguageHandler.ChooseLanguage(user.Language, "Thisday", "Четверг"), LanguageHandler.ChooseLanguage(user.Language, "Th", "Чт") },
				{ LanguageHandler.ChooseLanguage(user.Language, "Friday", "Пятница"), LanguageHandler.ChooseLanguage(user.Language, "Fr", "Пят") },
				{ LanguageHandler.ChooseLanguage(user.Language, "Saturday", "Суббота"), LanguageHandler.ChooseLanguage(user.Language, "Sat", "Сб") },
				{ LanguageHandler.ChooseLanguage(user.Language, "Sunday", "Воскресенье"), LanguageHandler.ChooseLanguage(user.Language, "Sun", "Вос") }
			}));
			var	lastDay = beginMonthDay.Day <= endMonthDay.Day;
			var	calendarRows = (((int)beginMonthDay.DayOfWeek - 1) + endMonthDay.Day) / 7 + 1;

			for (int i = 0; i < calendarRows; ++i)
			{
				var	days = new Dictionary<string, string>();

				for (int j = 1; j <= 7; ++j)
				{
					if ((i + 1) * j >= (int)beginMonthDay.DayOfWeek && lastDay)
					{
						days.Add($"Date {beginMonthDay.ToShortDateString()}", beginMonthDay.Day.ToString());
						if (beginMonthDay.Day != endMonthDay.Day)
						{
							beginMonthDay = beginMonthDay.AddDays(1);
							lastDay = beginMonthDay.Day <= endMonthDay.Day;
						}
						else
							lastDay = false;
					}
					else
						days.Add($"{LanguageHandler.ChooseLanguage(user.Language, "Empty", "Пусто")} {j * (i + 1)}", " ");
				}
				inlineKeyboard.Add(InlineKeyboard.GetInlineKeyboardLine(days));
			}
			inlineKeyboard.Add(InlineKeyboard.GetInlineKeyboardLine(new Dictionary<string, string>
			{
				{ "save-date", $"{LanguageHandler.ChooseLanguage(user.Language, "Save", "Сохранить")} {Stickers.Save}" },
				{ "cancel-date", $"{LanguageHandler.ChooseLanguage(user.Language, "Cancel", "Отменить")} {Stickers.Cancel}" }
			}));
			inlineKeyboard.Add(InlineKeyboard.GetInlineKeyboardLine(new Dictionary<string, string>
			{
				{ "clear-date", $"{LanguageHandler.ChooseLanguage(user.Language, "Clear", "Очистить")} {Stickers.Clear}" }
			}));
			return (inlineKeyboard);
		}
    }
}