using System.Threading.Tasks;
using CultureEventsBot.Persistance;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CultureEventsBot.Core.Commands
{
	public abstract class Command
    {
        public abstract string	Name { get; }
		public abstract Task	Execute(Message message, TelegramBotClient client, DataContext context);
		public abstract bool	Contains(Message message);
		public abstract Task	Inline(CallbackQuery callbackQuery, TelegramBotClient client, DataContext context);
    }
}