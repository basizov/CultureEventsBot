using System.Linq;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Domain.Entities;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Inlines
{
	public class	FavouriteInline : Inline
	{
		public override string	Name => "fav,rem";

		public override async Task	ExecuteAsync(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context)
		{
			var chatId = callbackQuery.Message.Chat.Id;
			var user = await context.Users
				.Include(u => u.Favourites)
				.FirstOrDefaultAsync(u => u.ChatId == chatId);
			var	action = callbackQuery.Data;
			var	textId = callbackQuery.Message.Caption.Split("\n")[0];
			var	favourite = await GetFavouriteFromContext(context, int.Parse(textId));

			if (action == "fav")
				await AddToFavourite(user, favourite, callbackQuery, client);
			else if (action == "rem")
				await RemoveFromFavourite(user, favourite, callbackQuery, client);
			await context.SaveChangesAsync();
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

		private async Task	AddToFavourite(Domain.Entities.User user, Favourite favourite, CallbackQuery callbackQuery, TelegramBotClient client)
		{
			if (user.Favourites.FirstOrDefault(f => f.Id == favourite.Id) == null)
			{
				user.Favourites.Add(favourite);
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Added to favourites", "Добавлено в избранное")}"
				);
			}
			else
			{
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Already in favourites", "Было ранее добавлено в избранное")}"
				);
			}
		}
		private async Task	RemoveFromFavourite(Domain.Entities.User user, Favourite favourite, CallbackQuery callbackQuery, TelegramBotClient client)
		{
			if (user.Favourites.FirstOrDefault(f => f.Id == favourite.Id) != null)
			{
				user.Favourites.Remove(favourite);
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Removed from favourites", "Удалено из избранного")}"
				);
			}
			else
			{
				await client.AnswerCallbackQueryAsync(
					callbackQuery.Id,
					$"{LanguageHandler.ChooseLanguage(user.Language, "Already removed", "Уже удалено")}"
				);
			}
		}
		private async Task<Favourite>	GetFavouriteFromContext(DataContext context, int textId)
		{
			Favourite	ev = await context.Events.FirstOrDefaultAsync(e => e.Id == textId);

			if (ev == null)
				ev = await context.Films.FirstOrDefaultAsync(e => e.Id == textId);
			if (ev == null)
				ev = await context.Places.FirstOrDefaultAsync(e => e.Id == textId);
			return (ev);
		}
	}
}