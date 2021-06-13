using System.Collections.Generic;

namespace CultureEventsBot.Domain.Entities
{
    public class Film : Favourite
    {
		public ICollection<Genre>	Genres { get; set; } = new List<Genre>();
    }
}