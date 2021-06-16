using System.Threading.Tasks;
using CultureEventsBot.Persistance;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Commands
{
	public class	DateCommand : Command
	{
		public override string	Name => "Date,Дата";

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
		public override async Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context)
		{
			throw new System.NotImplementedException();
		}
	}
}