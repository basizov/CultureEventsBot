using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Entities;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Inlines
{
	public class	FilterInline : Inline
	{
		public override string	Name => $"{Stickers.NotChoosen},{Stickers.Choosen}";

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
			var	splitFilterCategory = callbackQuery.Data.Split(" ")[0];
			var	icon = callbackQuery.Data.Contains(Stickers.Choosen) ? Stickers.NotChoosen : Stickers.Choosen;
			var	categories = await context.Categories
				.Where(c => c.ChoosePlan == user.ChoosePlan)
				.OrderBy(c => c.Name)
				.ToListAsync();
			var	selectedCategory = categories.FirstOrDefault(c => c.Name.Contains(splitFilterCategory));

			if (selectedCategory != null)
			{
				selectedCategory.IsChecked = true;
				await context.SaveChangesAsync();
				foreach (var category in categories)
				{
					if (category.IsChecked)
						category.Name = $"{category.Name} {icon}";
					else
						category.Name = $"{category.Name} {Stickers.NotChoosen}";
				}
				categories.AddRange(new List<Category>
				{
					new Category { Name = $"{LanguageHandler.ChooseLanguage(user.Language, "Save", "Сохранить")} {Stickers.Save}" },
					new Category { Name = $"{LanguageHandler.ChooseLanguage(user.Language, "Cancel", "Отменить")} {Stickers.Cancel}" }
				});
				await client.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, callbackQuery.Message.Text, replyMarkup: new InlineKeyboardMarkup(InlineKeyboard.GetInlineKeyboard(2, categories.Select(c => c.Name).ToArray())));
				await client.AnswerCallbackQueryAsync(callbackQuery.Id, $"{splitFilterCategory} {LanguageHandler.ChooseLanguage(user.Language, "is choosen", "выбрана")}");
			}
			else
				await client.AnswerCallbackQueryAsync(callbackQuery.Id, $"{LanguageHandler.ChooseLanguage(user.Language, "There isn't this category:", "Данной категории не существует:")} {splitFilterCategory}");
		}
	}
}