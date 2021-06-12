using System.Collections.Generic;

namespace CultureEventsBot.Domain.Entities
{
    public class EventParent
    {
        public List<EventId> Results { get; set; }
		public int Count { get; set; }
    }

	public class EventId
	{
        public int	Id { get; set; }
	}
}