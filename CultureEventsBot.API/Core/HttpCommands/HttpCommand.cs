using System.Net.Http;
using System.Threading.Tasks;
using CultureEventsBot.Persistance;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CultureEventsBot.API.Core.HttpCommands
{
    public abstract class	HttpCommand
    {
        public abstract string	Name { get; }
		
		public abstract Task	ExecuteAsync(IHttpClientFactory httpClient, Message message, TelegramBotClient client, DataContext context, int pageSize = 1);
		public virtual bool	Contains(Message message)
		{
			var	res = message != null && message.Type == MessageType.Text;

			res = res && message.Text.Contains(Name);
			return (res);
		}
    }
}