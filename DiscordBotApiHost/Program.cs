


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
            WebApplication app;


            // global register services and their life span
            startup.ConfigureServices(builder.Services);
            // create web app
            app = builder.Build();

            startup.Configure(app, builder.Environment);


            app.Run();
        }
    }
}