using CultureEventsBot.Domain.Enums;

namespace CultureEventsBot.Domain.Entities
{
  public class Category
  {
    public string	Name { get; set; }
    public string	Slug { get; set; }
		public bool	IsChecked { get; set; }
		public EChoosePlan	ChoosePlan { get; set; }
  }
}