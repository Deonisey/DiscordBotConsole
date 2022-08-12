using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;

namespace DiscordBotApiHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            var discordConfig = new DiscordSocketConfig() 
            {
                MessageCacheSize = 1000
            };

            services
                .AddSingleton(Configuration)
                .AddLogging()
                .AddSingleton<DiscordSocketConfig>(discordConfig) //for clarity
                .AddSingleton<DiscordSocketClient>()
                .AddTransient<BaseCommandService>()
                .AddTransient<CommandService>()
                .AddSingleton<BotHostingService>();                
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //for now this is unnecessary, but you can configure generic request processing pipeline here
        }
    }    
}
