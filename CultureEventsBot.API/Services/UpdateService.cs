using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CultureEventsBot.API.Services
{
	public class UpdateService : IUpdateService
	{
		public Task ExecuteAsync(Update update)
		{
			throw new System.NotImplementedException();
			// https://api.telegram.org/bot1824858522:AAF9OvgfrVsLJA_DNnQPfJdZ5KCvtBFmCDE/setWebhook?url=https://7b3f5d1a6424.ngrok.io/api/update
		}
	}
}