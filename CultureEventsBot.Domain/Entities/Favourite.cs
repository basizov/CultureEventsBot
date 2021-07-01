using System.Collections.Generic;

namespace CultureEventsBot.Domain.Entities
{
  public class Favourite
  {
    public int	Id { get; set; }
		public string	Title { get; set; }
		public string	Description { get; set; }
		public string	Site_Url { get; set; }
		public ICollection<ImageResponse>	Images { get; set; } = new List<ImageResponse>();
  }
}