using CultureEventsBot.API.Services;
using CultureEventsBot.Core.Core;
using CultureEventsBot.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
       		services.AddHttpClient();
        	services.AddSingleton<IBotService, BotService>();
        	services.Configure<BotConfiguration>(_config.GetSection("BotConfiguration"));
            services.AddControllers().AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
    		Bot.GetBotClientAsync(new BotConfiguration
			{
				Name = "",
				Key = "",
				Url = ""
			}).Wait();
        }
    }
}
