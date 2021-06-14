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
    }
}