using System;
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
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.API.Core.HttpCommands
{
	public class	ShowFilmsHttpCommand : HttpCommand
	{
		public override string	Name => "Next,Далее,Previous,Назад";

		public override bool	Contains(Message message, DataContext context = null)
		{
			var	res = message != null && message.Text != null;
			var	splitName = Name.Split(",");
			Domain.Entities.User user = null;

			if (res)
			{
				foreach (var name in splitName)
				{
					if (context != null)
						user = context.Users.FirstOrDefault(u => u.ChatId == message.Chat.Id);
					res = message.Text.Contains(name) && (user == null || user.ChoosePlan == EChoosePlan.Film);
					if (res) break ;
				}
			}
			return (res);
		}
		public override async Task	ExecuteAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, int pageSize = 1)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var	categories = user.Categories == null ? "" : $"&categories={JoinCategories(user.Categories)}";

			user.CurrentFilm += (message.Text.Contains("Next") || message.Text.Contains("Далее") ? 1 : -1);
			if (user.CurrentFilm < 0)
				user.CurrentFilm = 0;
			var page = user.CurrentFilm + 1;
			var filmsIds = await HttpWork<Parent>.SendRequestAsync(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/movies/?lang={ConvertStringToEnum(user.Language)}&location=kzn&page_size={pageSize}&page={page}{categories}", httpClient);
			if (filmsIds != null)
			{
				foreach (var id in filmsIds.Results)
				{
					var film = await GetFilmByIdAsync(id.Id, httpClient, user);

					if (film == null)
						break ;
					var films = await context.Films
						.Include(f => f.Genres)
						.ToListAsync();
					var	rnd = new Random();

					foreach (var genre in film.Genres)
						foreach (var f in films)
							while (f.Genres.FirstOrDefault(g => g.Id == genre.Id) != null)
								genre.Id = rnd.Next(0, 100000);
					await Send.SendPhotoAsync(message.Chat.Id, film.Images.First().Image, $@"
<i>{film.Id}</i>
<b>{film.Title}</b>
{film.Description}
<i>{film.Site_Url}</i>", client, replyMarkup: new InlineKeyboardMarkup(
						InlineKeyboard.GetInlineMatrix(1,
							InlineKeyboardButton.WithCallbackData(LanguageHandler.ChooseLanguage(user.Language, "Add to favourites", "Добавить в избранное"), "fav")
						)
					), parseMode: ParseMode.Html);
					if (context.Films.Include(f => f.Genres).FirstOrDefault(e => e.Id == film.Id) == null)
						context.Films.Add(film);
					if (pageSize > 1)
						++user.CurrentFilm;
				}
				if (pageSize > 1)
					--user.CurrentFilm;
			}
			context.SaveChanges();
		}

		private async Task<Film>	GetFilmByIdAsync(int id, IHttpClientFactory httpClient, Domain.Entities.User user)
		{
			var res = await HttpWork<Film>.SendRequestAsync(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/movies/{id}", httpClient);

			if (res != null)
			{
				res.Description = res.Description ?? "";
				res.Description = res.Description.Replace("<p>", "");
				res.Description = res.Description.Replace("</p>", "");
			}
			return (res);
		}
	}
}