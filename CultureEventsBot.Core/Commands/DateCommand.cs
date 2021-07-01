using System;
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

		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);

			user.FilterDate = DateTime.Now;
			await Send.SendMessageAsync(message.Chat.Id, $"{LanguageHandler.ChooseLanguage(user.Language, "Choose the planned date:", "Выберите запланированную дату:")} {Stickers.PlannedDate}", client, replyMarkup: new InlineKeyboardMarkup(InlineKeyboard.GetDateInlineKeyboard(user)));
			await context.SaveChangesAsync();
		}
	}
}