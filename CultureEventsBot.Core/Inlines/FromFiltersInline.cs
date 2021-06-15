using System.Linq;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Inlines
{
	public class	FromFiltersInline : Inline
	{
		public override string	Name => $"{Stickers.Save},{Stickers.Cancel}";

		public override bool	Contains(CallbackQuery callbackQuery)
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
		public override async Task	ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == callbackQuery.Message.Chat.Id);
			var	categories = context.Categories
				.Where(c => c.ChoosePlan == user.ChoosePlan && c.IsChecked)
				.OrderBy(c => c.Name)
				.Select(c => c.Slug)
				.ToArray();

			if (callbackQuery.Data.Contains(Stickers.Cancel))
			{
				await client.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, $"{LanguageHandler.ChooseLanguage(user.Language, "Filters are cancelated", "Фильтры отменены")} {Stickers.Stop}");
				await client.AnswerCallbackQueryAsync(callbackQuery.Id, LanguageHandler.ChooseLanguage(user.Language, "Filters are cancelated", "Фильтры отменены"));
			}
			else
			{
				await client.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, $"{LanguageHandler.ChooseLanguage(user.Language, "Filters are applied", "Фильтры применены")} {Stickers.Ok}");
				await client.AnswerCallbackQueryAsync(callbackQuery.Id, LanguageHandler.ChooseLanguage(user.Language, "Filters are applied", "Фильтры применены"));
				user.Categories = categories;
				await context.SaveChangesAsync();
			}
		}
	}
}