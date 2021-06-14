using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Persistance;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.API.Core.HttpCommands
{
	public class	WeatherHttpCommand : HttpCommand
	{
		public override string Name => "Weather,Погода";

		public override bool	Contains(Message message, DataContext context = null)
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
		public override async Task	ExecuteAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, int pageSize = 1)
		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
			var	headers = new Dictionary<string, string>();

			headers.Add("x-rapidapi-key", "afa9b0e4c3msh324f1563390aa2dp1bdf92jsn04bf886907db");
			headers.Add("x-rapidapi-host", "weatherapi-com.p.rapidapi.com");
			var weather = await HttpWork<Weather>.SendRequestWithHeadersAsync(HttpMethod.Get, $"https://weatherapi-com.p.rapidapi.com/forecast.json?q=Kazan&days=1", httpClient, headers);

			if (weather != null)
			{
				await client.SendTextMessageAsync(message.Chat.Id, $@"{LanguageHandler.ChooseLanguage(user.Language, "Weather for", "Погода на")} {weather.Current.Last_Updated}:
{LanguageHandler.ChooseLanguage(user.Language, "Temperature is", "Температура")} {weather.Current.Temp_C} {LanguageHandler.ChooseLanguage(user.Language, "degrees", "градуса")}
{LanguageHandler.ChooseLanguage(user.Language, "Feels like ", "Ощущается как")} {weather.Current.Feelslike_C} {LanguageHandler.ChooseLanguage(user.Language, "degrees", "градуса")}
{LanguageHandler.ChooseLanguage(user.Language, "Wind speed is", "Скорость ветра:")} {weather.Current.Wind_Kph} {LanguageHandler.ChooseLanguage(user.Language, "km/h", "км/ч")}
{LanguageHandler.ChooseLanguage(user.Language, "Cloud is", "Облачность")} {weather.Current.Cloud}%
{LanguageHandler.ChooseLanguage(user.Language, "Today is", "Сегодня")} {weather.Current.Condition.Text}
				");
			}
		}
	}
}