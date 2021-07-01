using System.Threading.Tasks;
using CultureEventsBot.Persistance;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CultureEventsBot.Core.Commands
{
	public abstract class	Command
  {
    public abstract string	Name { get; }
  
		public abstract Task	ExecuteAsync(Message message, TelegramBotClient client, DataContext context);
		public virtual bool	Contains(Message message)
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
  }
}