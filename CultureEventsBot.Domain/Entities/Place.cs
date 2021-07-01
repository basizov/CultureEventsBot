namespace CultureEventsBot.Domain.Entities
{
  public class Place : Favourite
  {
    public string	Address { get; set; }
		public string	Timetable { get; set; }
		public string	Phone { get; set; }
		public string[]	Categories { get; set; }
  }
}