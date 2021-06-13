using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CultureEventsBot.Core.Commands;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Domain.Entities;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.API.Core
{
	public static class HttpExecute
    {
        public async static Task	ShowEventsAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, IReadOnlyList<Command> commands, int pageSize)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var page = user.CurrentEvent + 1;
			var eventsIds = await HttpWork<EventParent>.SendRequestAsync($"https://kudago.com/public-api/v1.4/events/?lang={ConvertStringToEnum(user.Language)}&location=kzn&page_size={pageSize}&page={page}", httpClient);

			if (eventsIds != null)
			{
				foreach (var id in eventsIds.Results)
				{
					var ev = await HttpWork<Event>.SendRequestAsync($"https://kudago.com/public-api/v1.4/events/{id.Id}", httpClient);

					if (ev == null)
						continue ;
					if (ev.Description != null && ev.Description.Length > 8)
					{
						ev.Description = ev.Description.Remove(0, 3);
						ev.Description = ev.Description.Remove(ev.Description.Length - 5);
					}
					else
						ev.Description = "";
					if (ev.Price == null || ev.Price == "")
						ev.Price = $"{LanguageHandler.ChooseLanguage(user.Language, "Unknown", "Неизвестно")}";
					var mes = await Send.SendPhotoAsync(message.Chat.Id, ev.Images.First().Image, $@"
<i>{ev.Id}</i>
<b>{ev.Title}</b>
<b>{LanguageHandler.ChooseLanguage(user.Language, "Price", "Цена")}: {(ev.Is_Free ? $"{LanguageHandler.ChooseLanguage(user.Language, "Free", "Бесплатно")}" : ev.Price)}</b>
{ev.Description}
<i>{ev.Site_Url}</i>", new InlineKeyboardMarkup(new[]
						{
							new []
							{
								InlineKeyboardButton.WithCallbackData($"{LanguageHandler.ChooseLanguage(user.Language, "Add to favourites", "Добавить в избранное")}", "fav")
							}
						}), ParseMode.Html, client);
					
					if (context.Events.FirstOrDefault(e => e.Id == ev.Id) == null)
						context.Events.Add(ev);
					++user.CurrentEvent;
				}
				context.SaveChanges();
			}
		}
        public async static Task	FavouritesAsync(Message message, TelegramBotClient client, DataContext context, IReadOnlyList<Command> commands)
		{
			var user = await context.Users
				.Include(u => u.Favourites)
				.ThenInclude(u => u.Images)
				.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var userEvents = user.Favourites;

			if (userEvents.Count > 0)
				await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Favourites:", "Избранные:"), client);
			else
				await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "You don't have favourites events yet :(", "У вас нет избранных событий:("), client);
			foreach (var ev in userEvents)
			{
				var mes = await client.SendPhotoAsync(message.Chat.Id,
					photo: ev.Images.First().Image,
					caption: $@"
<i>{ev.Id}</i>
<b>{ev.Title}</b>
{ConvertFavouriteToEvent(user, ev)}
{ev.Description}
<i>{ev.Site_Url}</i>",
					replyMarkup: new InlineKeyboardMarkup(new[]
					{
						new []
						{
							InlineKeyboardButton.WithCallbackData(LanguageHandler.ChooseLanguage(user.Language, "Remove from favourites", "Удалить из избранных"), "rem")
						}
					}),
					parseMode: ParseMode.Html
				);
			}
		}
        public async static Task	WeatherAsync(Message message, TelegramBotClient client, IHttpClientFactory httpClient, DataContext context)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var request = new HttpRequestMessage(HttpMethod.Get, $"https://weatherapi-com.p.rapidapi.com/forecast.json?q=Kazan&days=1");

			request.Headers.Add("x-rapidapi-key", "afa9b0e4c3msh324f1563390aa2dp1bdf92jsn04bf886907db");
			request.Headers.Add("x-rapidapi-host", "weatherapi-com.p.rapidapi.com");
			var clientHttp = httpClient.CreateClient();
			var response = await clientHttp.SendAsync(request);
			var weather = await response.Content.ReadFromJsonAsync<Weather>();

			request = new HttpRequestMessage(HttpMethod.Post, $"https://google-translate1.p.rapidapi.com/language/translate/v2");
			request.Headers.Add("x-rapidapi-host", "google-translate1.p.rapidapi.com");
			request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "q", weather.Current.Condition.Text },
				{ "target", "ru" },
				{ "source", "en" },
			});
			response = await clientHttp.SendAsync(request);
			var conditionText = await response.Content.ReadFromJsonAsync<Data>();

			await client.SendTextMessageAsync(message.Chat.Id, $@"{LanguageHandler.ChooseLanguage(user.Language, "Weather for", "Погода на")} {weather.Current.Last_Updated}:
{LanguageHandler.ChooseLanguage(user.Language, "Temperature is", "Температура")} {weather.Current.Temp_C}
{LanguageHandler.ChooseLanguage(user.Language, "Feels like ", "Ощущается как")} {weather.Current.Feelslike_C}
{LanguageHandler.ChooseLanguage(user.Language, "Wind speed is", "Скорость ветра:")} {weather.Current.Wind_Kph}
{LanguageHandler.ChooseLanguage(user.Language, "Cloud is", "Облачность")} {weather.Current.Cloud}
{conditionText?.Tranlations?.TranslatedText ?? weather.Current.Condition.Text}
");
		}
        public async static Task	AdminAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var users = context.Users;
			var	user = await users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);

			if (user.IsAdmin && user.IsAdminWritingPost)
			{
				foreach	(var u in users)
					if (u.MayNotification && !u.IsAdmin)
						await client.ForwardMessageAsync(u.ChatId, message.Chat.Id, message.MessageId);
				user.IsAdminWritingPost = false;
				await context.SaveChangesAsync();
				await Send.SendMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Success sending", "Успешное отправление"), client);
			}
		}
        public async static Task	CategoriesAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var keyboard = new List<IEnumerable<InlineKeyboardButton>>();
			var keyboardButtons = new List<InlineKeyboardButton>();

			foreach (var e in context.Events)
			{
				if (e.Categories != null)
				{
					for (int i = 0; i < e.Categories.Length; ++i)
					{
						var btn = new InlineKeyboardButton
						{
							Text = e.Categories[i],
							CallbackData = e.Categories[i]
						};

						if (keyboardButtons.FirstOrDefault(b => b.Text == e.Categories[i]) == null)
							keyboardButtons.Add(btn);
					}
				}
			}
			foreach (var item in keyboardButtons)
			{
				var tempKeyboardButtons = new List<InlineKeyboardButton>();

				tempKeyboardButtons.Add(item);
				keyboard.Add(tempKeyboardButtons);
			}
			await client.SendTextMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Choose the category:", "Выберите категорию:"), replyMarkup: new InlineKeyboardMarkup(keyboard));
		}
        public async static Task	ShowFilmsAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, IReadOnlyList<Command> commands, int pageSize)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var page = user.CurrentFilm + 1;
			var filmsIds = await HttpWork<EventParent>.SendRequestAsync($"https://kudago.com/public-api/v1.4/movies/?page_size={pageSize}&page={page}", httpClient);
			var random = new Random();
			
			if (filmsIds != null)
			{
				foreach (var id in filmsIds.Results)
				{
					var fil = await HttpWork<Film>.SendRequestAsync($"https://kudago.com/public-api/v1.4/movies/{id.Id}", httpClient);

					if (fil == null)
						continue ;
					foreach (var genre in fil.Genres)
					{
						var idx = random.Next(0, 1000000);

						genre.Id = idx;
					}
					if (fil.Description != null && fil.Description.Length > 8)
					{
						fil.Description = fil.Description.Remove(0, 3);
						fil.Description = fil.Description.Remove(fil.Description.Length - 5);
					}
					else if (fil.Description == null)
						fil.Description = "";
					var mes = await Send.SendPhotoAsync(message.Chat.Id, fil.Images.First().Image, $@"
<i>{fil.Id}</i>
<b>{fil.Title}</b>
{fil.Description}
<i>{fil.Site_Url}</i>", new InlineKeyboardMarkup(new[]
						{
							new []
							{
								InlineKeyboardButton.WithCallbackData($"{LanguageHandler.ChooseLanguage(user.Language, "Add to favourites", "Добавить в избранное")}", "fav")
							}
						}), ParseMode.Html, client);
					
					if (context.Films.FirstOrDefault(e => e.Id == fil.Id) == null)
						context.Films.Add(fil);
					++user.CurrentFilm;
				}
				context.SaveChanges();
			}
		}
		public async static Task	GenresAsync(Message message, TelegramBotClient client, DataContext context)
		{
			var	user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var keyboard = new List<IEnumerable<InlineKeyboardButton>>();
			var keyboardButtons = new List<InlineKeyboardButton>();

			foreach (var e in context.Films.Include(f => f.Genres))
			{
				if (e.Genres != null)
				{
					foreach (var genre in e.Genres)
					{
						var btn = new InlineKeyboardButton
						{
							Text = genre.Name,
							CallbackData = genre.Name
						};

						if (keyboardButtons.FirstOrDefault(b => b.Text == genre.Name) == null)
							keyboardButtons.Add(btn);
					}
				}
			}
			foreach (var item in keyboardButtons)
			{
				var tempKeyboardButtons = new List<InlineKeyboardButton>();

				tempKeyboardButtons.Add(item);
				keyboard.Add(tempKeyboardButtons);
			}
			await client.SendTextMessageAsync(message.Chat.Id, LanguageHandler.ChooseLanguage(user.Language, "Choose the genre:", "Выберите жанр:"), replyMarkup: new InlineKeyboardMarkup(keyboard));
		}
		private static string ConvertStringToEnum(ELanguage value)
		{
			if (value == ELanguage.English)
				return ("en");
			return ("ru");
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