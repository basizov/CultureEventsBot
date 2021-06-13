using System.Threading.Tasks;
using CultureEventsBot.Persistance;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CultureEventsBot.Core.Commands
{
	public abstract class Command
    {
        public abstract string	Name { get; }
		
		public abstract Task	Execute(Message message, TelegramBotClient client, DataContext context);
		public virtual bool	Contains(Message message, string inline = null)
		{
			if (message == null || message.Type != MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}
    }
}