using System;
using System.Collections.Generic;
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
	public class	DateCommand : Command
	{
		public override string	Name => "Date,Дата";

		public override bool	Contains(Message message)
		{
			var	res = message != null && message.Text != null;
			var	splitName = Name.Split(",");

			if (res)
			{
				foreach (var name in splitName)
				{
					res = message.Text.Contains(name);
					if (res) break ;
				}
			}
			return (res);
		}
		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
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
			var	currentDate = DateTime.Now;
			var	beginMonthDay = new DateTime(currentDate.Year, currentDate.Month, 1);
			var	endMonthDay = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
			var inlineKeyboard = new List<IEnumerable<InlineKeyboardButton>>();
			var inlineKeyboardButtons = new List<InlineKeyboardButton>();
			// var	keyboard = new InlineKeyboardMarkup();

			// await Send.SendMessageAsync(message.Chat.Id, $"{LanguageHandler.ChooseLanguage(user.Language, "Chose the planned date:", "Выберите запланированную дату:")} {Stickers.PlannedDate} {currentDate}", client);
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

			for (int i = 0; i < ((int)beginMonthDay.DayOfWeek + endMonthDay.Day) / 7 + 1; ++i)
			{
				var	days = new Dictionary<string, string>();

				for (int j = 1; j <= 7; ++j)
				{
					if ((i + 1) * j >= (int)beginMonthDay.DayOfWeek && lastDay)
					{
						days.Add(beginMonthDay.ToShortDateString(), beginMonthDay.Day.ToString());
						if (beginMonthDay.Day != endMonthDay.Day)
						{
							beginMonthDay = beginMonthDay.AddDays(1);
							lastDay = beginMonthDay.Day <= endMonthDay.Day;
						}
						else
							lastDay = false;
					}
					else
						days.Add($"Empty {j * (i + 1)}", " ");
				}
				inlineKeyboard.Add(InlineKeyboard.GetInlineKeyboardLine(days));
			}
			await Send.SendMessageAsync(message.Chat.Id, $"{LanguageHandler.ChooseLanguage(user.Language, "Choose the planned date:", "Выберите запланированную дату:")} {Stickers.PlannedDate}", client, replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
		}
	}
}