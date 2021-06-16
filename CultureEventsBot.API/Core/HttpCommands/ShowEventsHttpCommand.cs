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
	public class	ShowEventsHttpCommand : HttpCommand
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
					res = message.Text.Contains(name) && (user == null || user.ChoosePlan == EChoosePlan.Event);
					if (res) break ;
				}
			}
			return (res);
		}
		public override async Task	ExecuteAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, int pageSize = 1)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var	categories = user.Categories == null || user.Categories.Length == 0 ? "" : $"&categories={JoinCategories(user.Categories)}";
			var	actualSince = user.BeginFilterDate == null ? "" : $"&actual_since={((DateTimeOffset)user.BeginFilterDate).ToUnixTimeSeconds()}";
			var	actualUntil = user.EndFilterDate == null ? "" : $"&actual_until={((DateTimeOffset)user.EndFilterDate).ToUnixTimeSeconds()}";

			user.CurrentEvent += (message.Text.Contains("Next") || message.Text.Contains("Далее") ? 1 : -1);
			if (user.CurrentEvent < 0)
				user.CurrentEvent = 0;
			var page = user.CurrentEvent + 1;
			var eventsIds = await HttpWork<Parent>.SendRequestAsync(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/events/?lang={ConvertStringToEnum(user.Language)}&location=kzn&page_size={pageSize}&page={page}{categories}{actualSince}{actualUntil}", httpClient);
			if (eventsIds != null)
			{
				foreach (var id in eventsIds.Results)
				{
					var ev = await GetEventByIdAsync(id.Id, httpClient, user);
					
					if (ev == null)
						break ;
					await Send.SendPhotoAsync(message.Chat.Id, ev.Images.First().Image, $@"
<i>{ev.Id}</i>
<b>{ev.Title}</b>
<b>{LanguageHandler.ChooseLanguage(user.Language, "Price", "Цена")}: {ev.Price}</b>
{ev.Description}
<i>{ev.Site_Url}</i>", client, replyMarkup: new InlineKeyboardMarkup(
						InlineKeyboard.GetInlineMatrix(1,
							InlineKeyboardButton.WithCallbackData(LanguageHandler.ChooseLanguage(user.Language, "Add to favourites", "Добавить в избранное"), "fav")
						)
					), parseMode: ParseMode.Html);
					if (context.Events.FirstOrDefault(e => e.Id == ev.Id) == null)
						context.Events.Add(ev);
					if (pageSize > 1)
						++user.CurrentEvent;
				}
				if (pageSize > 1)
					--user.CurrentEvent;
			}
			context.SaveChanges();
		}

		private async Task<Event>	GetEventByIdAsync(int id, IHttpClientFactory httpClient, Domain.Entities.User user)
		{
			var res = await HttpWork<Event>.SendRequestAsync(HttpMethod.Get, $"https://kudago.com/public-api/v1.4/events/{id}", httpClient);

			if (res != null)
			{
				res.Description = res.Description ?? "";
				res.Description = res.Description.Replace("<p>", "");
				res.Description = res.Description.Replace("</p>", "");
				res.Price = res.Is_Free ? LanguageHandler.ChooseLanguage(user.Language, "Free", "Бесплатно") : res.Price;
				if (res.Price == null || res.Price == "")
					res.Price = $"{LanguageHandler.ChooseLanguage(user.Language, "Unknown", "Неизвестно")}";
			}
			return (res);
		}
	}
}