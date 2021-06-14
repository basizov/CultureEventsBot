using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace CultureEventsBot.Core.Dialog
{
    public static class	Keyboard
    {
        public static IEnumerable<IEnumerable<KeyboardButton>>	GetKeyboard(int columns, params string[] keywords)
		{
			var keyboard = new List<IEnumerable<KeyboardButton>>();
			var keyboardButtons = new List<KeyboardButton>();

			foreach (var keyword in keywords)
				if (keyboardButtons.FirstOrDefault(b => b.Text == keyword) == null)
					keyboardButtons.Add(GetKeyboardButton(keyword));
			for (int i = 0; i < keyboardButtons.Count; i += columns)
			{
				var tempKeyboardButtons = new List<KeyboardButton>();

				for (int j = 0; j < columns && i + j < keyboardButtons.Count; ++j)
					tempKeyboardButtons.Add(keyboardButtons[i + j]);
				keyboard.Add(tempKeyboardButtons);
			}
			return (keyboard);
		}
        public static KeyboardButton	GetKeyboardButton(string keyword)
		{
			var res = new KeyboardButton(keyword);

			return (res);
		}
        public static IEnumerable<KeyboardButton>	GetKeyboardLine(params string[] keywords)
		{
			var res = new List<KeyboardButton>();

			foreach (var keyword in keywords)
				if (res.FirstOrDefault(b => b.Text == keyword) == null)
					res.Add(GetKeyboardButton(keyword));
			return (res);
		}
        public static IEnumerable<IEnumerable<KeyboardButton>>	GetKeyboardMatrix(params IEnumerable<KeyboardButton>[] keyboards)
		{
			var res = new List<IEnumerable<KeyboardButton>>();

			foreach (var keyboard in keyboards)
				res.Add(keyboard);
			return (res);
		}
    }
}