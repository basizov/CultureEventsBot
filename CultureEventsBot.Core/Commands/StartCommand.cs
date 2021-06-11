using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CultureEventsBot.Core.Commands
{
	public class StartCommand : Command
	{
		public override string Name => @"/start";

		public override async Task Execute(Message message, TelegramBotClient client)
		{
            var chatId = message.Chat.Id;

            await client.SendTextMessageAsync(chatId, "Hallo I'm ASP.NET Core Bot", parseMode: ParseMode.Markdown);
		}

		public override bool Contains(Message message)
		{
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
		}
	}
}