using System.Collections.Generic;

namespace CultureEventsBot.Domain.Entities
{
	public class Event
    {
        public int	Id { get; set; }
		public bool	Is_Free { get; set; }
		public string	Price { get; set; }
		// public float	Longitude { get; set; }
		// public float	Latitude { get; set; }
		public string	Title { get; set; }
		public string	Short_Title { get; set; }
		// public string	Place { get; set; }
		public string	Description { get; set; }
		public string	BodyText { get; set; }
		public string	Site_Url { get; set; }
		// public string	Age_Restriction { get; set; }
		public ICollection<ImageResponse>	Images { get; set; } = new List<ImageResponse>();
		public string[]	Categories { get; set; }
		// public DateTime	Dates { get; set; }
    }

	public class ImageResponse
	{
		public int	Id { get; set; }
		public string	Image { get; set; }
	}
}