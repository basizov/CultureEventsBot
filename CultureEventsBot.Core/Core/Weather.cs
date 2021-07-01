namespace CultureEventsBot.Core.Core
{
  public class Weather
  {
    public CurrentWeather Current { get; set; }
  }

	public class CurrentWeather
	{
		public double Temp_C { get; set; }
		public double Wind_Kph { get; set; }
		public double Feelslike_C { get; set; }
		public int Cloud { get; set; }
		public string Last_Updated { get; set; }
		public Condition	Condition { get; set; }
	}

	public class Condition
	{
		public string	Text { get; set; }
	}
}