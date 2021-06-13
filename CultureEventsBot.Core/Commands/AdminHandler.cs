using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Commands
{
	public class AdminHandler : Command
	{
		public override string Name => @"/admin";

		public override async Task Execute(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);

			if (user != null && (user.IsAdmin || user.UserName == "barkasOff")) // TODO: Delete
			{
				await client.SendTextMessageAsync(
					chatId: message.Chat.Id,
					text: $@"
{LanguageHandler.ChooseLanguage(user.Language, "Click button and the next message will be introduce to everyone is submitting", "Нажмите на кнопку и ваше следущее сообщение отправится всем подписанным на рассылку пользователям")}",
					replyMarkup: new InlineKeyboardMarkup(new[]
					{
						new []
						{
							InlineKeyboardButton.WithCallbackData($"{LanguageHandler.ChooseLanguage(user.Language, "Create a new post", "Создать новый пост")}", "new")
						}
					})
				);
			}
		}
	}
}