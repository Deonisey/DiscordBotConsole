using Discord;
using Discord.WebSocket;

namespace DiscordBotApiHost
{
    public class BotHostingService : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly BaseCommandService _commandService;
        private readonly ILogger<BotHostingService> _logger;
        private readonly IConfiguration _configuration;
        private readonly CmdBotConf _botConf;

        public BotHostingService(DiscordSocketClient client, BaseCommandService commandService, ILogger<BotHostingService> logger,IConfiguration configuration, CmdBotConf cmdBotConf)
        {
            _client = client;
            _commandService = commandService;
            _logger = logger;
            _configuration = configuration;
            _botConf = cmdBotConf;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine();
            Console.WriteLine("Token: " + _configuration.GetValue<string>("Token"));
            await _client.LoginAsync(TokenType.Bot, _configuration.GetValue<string>("Token")); //todo: configure appsettings
            await _client.StartAsync();

            await _commandService.InstallCommandsAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting shutdown");
            await _client.StopAsync();

            _logger.LogInformation("Connection closed, exiting");
        }
    }
}
