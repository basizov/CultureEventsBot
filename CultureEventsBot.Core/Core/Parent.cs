using System.Collections.Generic;

namespace CultureEventsBot.Core.Core
{
    public class Parent
    {
        public List<ParentId> Results { get; set; }
		public int Count { get; set; }
    }

	public class ParentId
	{
        public int	Id { get; set; }
	}
}