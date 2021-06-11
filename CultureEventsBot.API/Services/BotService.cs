using CultureEventsBot.Domain.Entities;
using Microsoft.Extensions.Options;

namespace CultureEventsBot.API.Services
{
	public class BotService : IBotService
	{
		private readonly BotConfiguration	_config;
		public BotConfiguration Configuration => _config;

		public BotService(IOptions<BotConfiguration> config) =>
			_config = config.Value;
	}
}