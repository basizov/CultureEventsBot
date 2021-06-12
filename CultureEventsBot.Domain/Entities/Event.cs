using System;
using System.Collections.Generic;

namespace CultureEventsBot.Domain.Entities
{
	public class Event
    {
        public int	Id { get; set; }
		public bool	IsFree { get; set; }
		public string	Price { get; set; }
		public float	Longitude { get; set; }
		public float	Latitude { get; set; }
		public string	Title { get; set; }
		public string	ShortTitle { get; set; }
		// public string	Place { get; set; }
		public string	Description { get; set; }
		public string	BodyText { get; set; }
		public string	SiteUrl { get; set; }
		public string	AgeRestriction { get; set; }
		public ICollection<ImageResponse>	Images { get; set; } = new List<ImageResponse>();
		// public IEnumerable<string>	Categories { get; set; } = new List<string>();
		// public DateTime	Dates { get; set; }
    }

	public class ImageResponse
	{
		public int	Id { get; set; }
		public string	Image { get; set; }
	}
}