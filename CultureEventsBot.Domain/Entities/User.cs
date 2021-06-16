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
		public bool IsAdminWritingPost { get; set; }
		public ELanguage	Language { get; set; }
		public EStatus	Status { get; set; }
		public bool	MayNotification { get; set; } = true;
		public int	CurrentEvent { get; set; }
		public int	CurrentFilm { get; set; }
		public int	CurrentPlace { get; set; }
		public EChoosePlan	ChoosePlan { get; set; }
		public string[]	Categories { get; set; }
		public DateTime	FilterDate { get; set; }
		public DateTime?	BeginFilterDate { get; set; }
		public DateTime?	EndFilterDate { get; set; }
		public DateTime?	NewBeginFilterDate { get; set; }
		public DateTime?	NewEndFilterDate { get; set; }
		public ICollection<Favourite>	Favourites { get; set; } = new List<Favourite>();
    }
}