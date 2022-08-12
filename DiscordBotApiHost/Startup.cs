using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace DiscordBotApiHost
{
    public class Startup
    {
        public IConfiguration Configuration;


        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            //Configuration.Bind(builder)
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            var discordConfig = new DiscordSocketConfig() 
            {
                MessageCacheSize = 1000
            };

            var json = File.ReadAllText(Directory.GetCurrentDirectory() + "/data.json");

            CmdBotConf? cmdBotConf;
            cmdBotConf = JsonConvert.DeserializeObject<CmdBotConf>(json);
            if (cmdBotConf == null) throw new Exception("Wrong json");

            // registering service descriptors
            services
                .AddSingleton(Configuration)
                .AddLogging()
                .AddSingleton<DiscordSocketConfig>(discordConfig) //for clarity
                .AddSingleton<DiscordSocketClient>()
                .AddTransient<BaseCommandService>()
                .AddTransient<CommandService>()
                .AddSingleton<BotHostingService>()
                .AddSingleton<CmdBotConf>(cmdBotConf);      
        }

        
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // pattern locator
            BotHostingService hostingService = app.ApplicationServices.GetRequiredService<BotHostingService>();

            await hostingService.StartAsync(CancellationToken.None);
            //for now this is unnecessary, but you can configure generic request processing pipeline here
        }
    }    
}
