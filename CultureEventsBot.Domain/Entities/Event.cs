using System.Collections.Generic;

namespace CultureEventsBot.Domain.Entities
{
	public class Event : Favourite
    {
		public bool	Is_Free { get; set; }
		public string	Price { get; set; }
		public string	Short_Title { get; set; }
		public string	BodyText { get; set; }
		public string[]	Categories { get; set; }
    }
}