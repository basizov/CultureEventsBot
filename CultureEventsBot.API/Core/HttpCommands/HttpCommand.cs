using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.Domain.Enums;
using CultureEventsBot.Persistance;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.API.Core.HttpCommands
{
  public abstract class	HttpCommand
  {
    public abstract string	Name { get; }
		
		public abstract Task	ExecuteAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, int pageSize = 1);
		public virtual bool	Contains(Message message, DataContext context = null)
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

		protected string	JoinCategories(string[] categories)
		{
			var	res = "";

			foreach (var category in categories)
				res += $"{category},";
			if (res.Last() == ',')
				res = res.Remove(res.Length - 1);
			return (res);
		}
		protected string	ConvertStringToEnum(ELanguage value)
		{
			if (value == ELanguage.English)
				return ("en");
			return ("ru");
		}
  }
}