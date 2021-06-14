using System.Linq;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Core.Dialog;
using CultureEventsBot.Domain.Entities;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Commands
{
	public class	FavouriteCommand : Command
	{
		public override string	Name => "Favourites,Избранное";

		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var user = await context.Users
				.Include(u => u.Favourites)
				.ThenInclude(u => u.Images)
				.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);

			if (user.Favourites.Count == 0)
				await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "You don't have favourites yet :(", "У вас нет избранного:("), client);
			else
			{
				foreach (var fav in user.Favourites)
				{
					await Send.SendPhotoAsync(message.Chat.Id, fav.Images.First().Image, $@"
<i>{fav.Id}</i>
<b>{fav.Title}</b>
{ConvertFavouriteToEvent(user, fav)}
{fav.Description}
<i>{fav.Site_Url}</i>", client, replyMarkup: new InlineKeyboardMarkup(
						InlineKeyboard.GetInlineMatrix(1,
							InlineKeyboardButton.WithCallbackData(LanguageHandler.ChooseLanguage(user.Language, "Remove from favourites", "Удалить из избранных"), "rem")
						)
					), parseMode: ParseMode.Html);
				}
			}
		}
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

		private static string	ConvertFavouriteToEvent(Domain.Entities.User user, Favourite favourite)
		{
			var res = "";

			if (favourite is Event ev)
				res = $"<b>{LanguageHandler.ChooseLanguage(user.Language, "Price", "Цена")}: {(ev.Is_Free ? $"{LanguageHandler.ChooseLanguage(user.Language, "Free", "Бесплатно")}" : ev.Price)}</b>";
			return (res);
		}
	}
}