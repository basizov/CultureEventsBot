using System.Collections.Generic;
using System.Threading.Tasks;
using CultureEventsBot.Core.Commands;
using Telegram.Bot;
using CultureEventsBot.Core.Inlines;
using CultureEventsBot.Core.Core;
using CultureEventsBot.API.Core.HttpCommands;

namespace CultureEventsBot.API.Core
{
	public class Bot
    {
        private static TelegramBotClient botClient;
        private static List<Command> commandsList;
        private static List<HttpCommand> htppCommandsList;
        private static List<Inline> inlineList;

        public static IReadOnlyList<Command> Commands => commandsList?.AsReadOnly();
        public static IReadOnlyList<HttpCommand> HtppCommands => htppCommandsList?.AsReadOnly();
        public static IReadOnlyList<Inline> Inlines => inlineList?.AsReadOnly();

		public static async Task<TelegramBotClient> GetBotClientAsync(BotConfiguration config)
		{
			if (botClient != null)
				return (botClient);
			
            commandsList = new List<Command>();
            commandsList.Add(new AdminCommand());
            commandsList.Add(new FavouriteCommand());
            commandsList.Add(new InfoCommand());
            commandsList.Add(new KeyboardCommand());
            commandsList.Add(new LanguageCommand());
            commandsList.Add(new MenuCommand());
            commandsList.Add(new RuleCommand());
            commandsList.Add(new StartCommand());
            commandsList.Add(new SentencesCommand());
            commandsList.Add(new NavigationCommand());
            commandsList.Add(new DateCommand());

			htppCommandsList = new List<HttpCommand>();
            htppCommandsList.Add(new ShowEventsHttpCommand());
            htppCommandsList.Add(new ShowFilmsHttpCommand());
            htppCommandsList.Add(new ShowPlacesHttpCommand());
            htppCommandsList.Add(new WeatherHttpCommand());
            htppCommandsList.Add(new FiltersCommand());

            inlineList = new List<Inline>();
            inlineList.Add(new AdminInline());
            inlineList.Add(new FavouriteInline());
            inlineList.Add(new LanguageInline());
            inlineList.Add(new FilterInline());
            inlineList.Add(new FromFiltersInline());
            inlineList.Add(new DateInline());
            inlineList.Add(new DateNavInline());
            inlineList.Add(new DateSureInline());

            botClient = new TelegramBotClient(config.Key);
            var hook = string.Format(config.Url, "api/message/update");

            await botClient.SetWebhookAsync(hook);
			return (botClient);
		}
    }
}