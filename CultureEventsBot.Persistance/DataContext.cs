using CultureEventsBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CultureEventsBot.Persistance
{
  public class	DataContext : DbContext
  {
    public DbSet<User>	Users { get; set; }
    public DbSet<Event>	Events { get; set; }
    public DbSet<Film>	Films { get; set; }
    public DbSet<Place>	Places { get; set; }
    public DbSet<Category>	Categories { get; set; }
    
		// docker run --name dev -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=secret -p 5432:5432 -d postgres:latest
    // https://api.telegram.org/bot1824858522:AAF9OvgfrVsLJA_DNnQPfJdZ5KCvtBFmCDE/setWebhook?url=https://b93c27d2262f.ngrok.io/api/update
		public DataContext(DbContextOptions options) : base(options) { }

		protected override void	OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasKey(c => c.Name);
		}
  }
}