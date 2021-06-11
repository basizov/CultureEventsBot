using CultureEventsBot.Domain.Entities;

namespace CultureEventsBot.API.Services
{
    public interface IBotService
    {
    	BotConfiguration Configuration { get; }
    }
}