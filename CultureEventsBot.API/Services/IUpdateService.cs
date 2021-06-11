using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CultureEventsBot.API.Services
{
    public interface IUpdateService
    {
    	Task ExecuteAsync(Update update);
    }
}