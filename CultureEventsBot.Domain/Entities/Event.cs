using System;

namespace CultureEventsBot.Domain.Entities
{
    public class Event
    {
        public Guid	Id { get; set; }
		public bool	IsFree { get; set; }
		public double	Price { get; set; }
		public float	Longitude { get; set; }
		public float	Latitude { get; set; }
		public string	Title { get; set; }
		public string	Address { get; set; }
		public DateTime	Time { get; set; }
    }
}