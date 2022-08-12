


namespace DiscordBotApiHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebApplication(args);            
        }

        public static void CreateWebApplication(string[] args)
        {
            
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            Startup startup = new Startup();
            WebApplication app = builder.Build();

            startup.ConfigureServices(builder.Services);
            startup.Configure(app, builder.Environment);

            app.Run();
        }
    }
}