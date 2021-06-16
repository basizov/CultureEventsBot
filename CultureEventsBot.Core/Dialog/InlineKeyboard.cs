using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Dialog
{
	public static class	InlineKeyboard
    {
        public static IEnumerable<IEnumerable<InlineKeyboardButton>>	GetInlineKeyboard(int columns, params string[] keywords)
		{
			var inlineKeyboard = new List<IEnumerable<InlineKeyboardButton>>();
			var inlineKeyboardButtons = new List<InlineKeyboardButton>();

			foreach (var keyword in keywords)
			{
				if (inlineKeyboardButtons.FirstOrDefault(b => b.Text == keyword) == null)
				{
					var btn = new InlineKeyboardButton
					{
						Text = keyword,
						CallbackData = keyword
					};

					inlineKeyboardButtons.Add(btn);
				}
			}
			for (int i = 0; i < inlineKeyboardButtons.Count; i += columns)
			{
				var tempKeyboardButtons = new List<InlineKeyboardButton>();

				for (int j = 0; j < columns && i + j < inlineKeyboardButtons.Count; ++j)
					tempKeyboardButtons.Add(inlineKeyboardButtons[i + j]);
				inlineKeyboard.Add(tempKeyboardButtons);
			}
			return (inlineKeyboard);
		}
        public static IEnumerable<IEnumerable<InlineKeyboardButton>>	GetInlineMatrix(int columns, params InlineKeyboardButton[] inlineKeyboardButtons)
		{
			var inlineKeyboard = new List<IEnumerable<InlineKeyboardButton>>();

			for (int i = 0; i < inlineKeyboardButtons.Length; i += columns)
			{
				var tempKeyboardButtons = new List<InlineKeyboardButton>();

				for (int j = 0; j < columns && i + j < inlineKeyboardButtons.Length; ++j)
					tempKeyboardButtons.Add(inlineKeyboardButtons[i + j]);
				inlineKeyboard.Add(tempKeyboardButtons);
			}
			return (inlineKeyboard);
		}
        public static IEnumerable<InlineKeyboardButton>	GetInlineKeyboardLine(Dictionary<string, string> words)
		{
			var res = new List<InlineKeyboardButton>();

			foreach (var word in words)
				if (res.FirstOrDefault(b => b.Text == word.Key) == null)
					res.Add(GetInlineKeyboardButton(word.Value, word.Key));
			return (res);
		}
        public static InlineKeyboardButton	GetInlineKeyboardButton(string value, string key)
		{
			var res = new InlineKeyboardButton
			{
				Text = value,
				CallbackData = key
			};

			return (res);
		}
    }
}