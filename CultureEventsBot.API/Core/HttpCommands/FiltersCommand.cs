using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Entities;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.API.Core.HttpCommands
{
	public class FiltersCommand : HttpCommand
	{
		public override string Name => "Filters,Фильтры";

		public override async Task ExecuteAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, int pageSize = 1)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			List<Category>	categories = new List<Category>();

			if (user.ChoosePlan == EChoosePlan.Event)
				categories = await GetCategoriesAsync(httpClient, user, "event-categories");
			else if (user.ChoosePlan == EChoosePlan.Place)
				categories = await GetCategoriesAsync(httpClient, user, "place-categories");

			foreach (var category in categories)
			{
				category.ChoosePlan = user.ChoosePlan;
				if (!context.Categories.Contains(category))
					context.Categories.Add(category);
			}
			await context.SaveChangesAsync();
			categories = await context.Categories
				.Where(c => c.ChoosePlan == user.ChoosePlan)
				.OrderBy(c => c.Name)
				.ToListAsync();
			
			foreach (var category in categories)
			{
				if (category.IsChecked)
					category.Name = $"{category.Name} {Stickers.Choosen}";
				else
					category.Name = $"{category.Name} {Stickers.NotChoosen}";
			}
			categories.AddRange(new List<Category>
			{
				new Category { Name = $"{LanguageHandler.ChooseLanguage(user.Language, "Save", "Сохранить")} {Stickers.Save}" },
				new Category { Name = $"{LanguageHandler.ChooseLanguage(user.Language, "Cancel", "Отменить")} {Stickers.Cancel}" }
			});
			await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Choose the categories:", "Выберите категории:"), client, new InlineKeyboardMarkup(InlineKeyboard.GetInlineKeyboard(2, categories.Select(c => c.Name).ToArray())));
		}

		private async Task<List<Category>>	GetCategoriesAsync(IHttpClientFactory httpClient, Domain.Entities.User user, string url)
		{
			var	categories = await HttpWork<List<Category>>.SendRequestAsync(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/{url}/?lang={ConvertStringToEnum(user.Language)}&location=kzn", httpClient);

			if (categories == null)
				categories = new List<Category>();
			return (categories);
		}
	}
}