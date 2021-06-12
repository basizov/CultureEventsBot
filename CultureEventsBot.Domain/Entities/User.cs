using System;
using System.Collections.Generic;
using CultureEventsBot.Domain.Enums;

namespace CultureEventsBot.Domain.Entities
{
    public class User
    {
		public Guid Id { get; set; }
        public string	FirstName { get; set; }
        public string	SecondName { get; set; }
        public string	UserName { get; set; }
		public long	ChatId { get; set; }
		public bool IsAdmin { get; set; }
		public ELanguage	Language { get; set; }
		public EStatus	Status { get; set; }
		public ICollection<Event>	Favourites { get; set; } = new List<Event>();
    }
}