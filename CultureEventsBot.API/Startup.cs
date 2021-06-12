using CultureEventsBot.API.Services;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Domain.Entities;
using CultureEventsBot.Persistance;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CultureEventsBot.API
{
	public class Startup
    {
        public readonly IConfiguration _config;

        public Startup(IConfiguration config) =>
            _config = config;

        public void ConfigureServices(IServiceCollection services)
        {
			services.AddLogging();
       		services.AddHttpClient();
        	services.AddSingleton<IBotService, BotService>();
        	services.Configure<BotConfiguration>(_config.GetSection("BotConfiguration"));
            services.AddControllers().AddNewtonsoftJson();
      		services.AddDbContext<DataContext>(opt => opt.UseNpgsql(_config.GetConnectionString("DefaultConnection")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
