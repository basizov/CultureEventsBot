using System.Collections.Generic;
using System.Threading.Tasks;
using CultureEventsBot.Core.Commands;
using CultureEventsBot.Domain.Entities;
using Telegram.Bot;
using CultureEventsBot.Persistance;

namespace CultureEventsBot.Core.Core
{
    public class Bot
    {
        private static TelegramBotClient botClient;
        private static List<Command> commandsList;

        public static IReadOnlyList<Command> Commands => commandsList?.AsReadOnly();

		public static async Task<TelegramBotClient> GetBotClientAsync(BotConfiguration config)
		{
			if (botClient != null)
				return (botClient);
			
            commandsList = new List<Command>();
            commandsList.Add(new StartCommand());
            commandsList.Add(new LanguageCommand());
            commandsList.Add(new EventsCommand());

            botClient = new TelegramBotClient(config.Key);
            string hook = string.Format(config.Url, "api/message/update");
            await botClient.SetWebhookAsync(hook);
			return (botClient);
		}
    }
}