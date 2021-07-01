using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Dialog
{
  public static class	Send
  {
    public static async Task<Message> SendMessageAsync(long chatId, string message, TelegramBotClient client, IReplyMarkup replyMarkup = null, ParseMode parseMode = ParseMode.Default) =>
      await client.SendTextMessageAsync(chatId: chatId, text: message, replyMarkup: replyMarkup, parseMode: parseMode);
    public static async Task<Message> SendPhotoAsync(long chatId, InputOnlineFile file, string caption, TelegramBotClient client, IReplyMarkup replyMarkup = null, ParseMode parseMode = ParseMode.Default) =>
      await client.SendPhotoAsync(chatId: chatId, photo: file, caption: caption, replyMarkup: replyMarkup, parseMode: parseMode);
  }
}