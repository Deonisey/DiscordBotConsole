using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Commands;
using System.Reflection;

namespace DiscordBotApiHost
{
    public class BaseCommandService : CommandService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BaseCommandService> _logger;
        private readonly CmdBotConf _botConf;

        public BaseCommandService(
            DiscordSocketClient client,
            CommandService commands,
            IServiceProvider serviceProvider,
            ILogger<BaseCommandService> logger,
            CmdBotConf botConf)
        {
            _client = client;
            _commands = commands;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _botConf = botConf;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: null
                );
        }

        public async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            SocketUserMessage? message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            try { await PrepareCommandContext(context, message, argPos); }
            catch (Exception e) { _logger.LogError(e.Message); }

            await _commands.ExecuteAsync(context, argPos, _serviceProvider);
        }

        private async Task PrepareCommandContext(SocketCommandContext commandContext, SocketUserMessage message, int argPos)
        {
            CmdBot cmdBot = new CmdBot(_botConf);
            try
            {
                switch (message.Content.Substring(argPos).Split()[0].ToLower())
                {
                    case "вопрос":
                        cmdBot.AnswerMagicBall(message, argPos + 6);
                        break;

                    case string s when (s == "wod" || s == "вод"):
                        cmdBot.Wod(message);
                        break;

                    case string s when (
                        s == "lvlup" || s == "levelup" ||
                        s == "лвлап" || s == "левелап"):
                        cmdBot.Levelup(message);
                        break;

                    case string s when (
                        s == "aura" || s == "auras" ||
                        s == "аура" || s == "ауры"):
                        cmdBot.Auras(message);
                        break;

                    case string s when (s == "commands" || s == "команды"):
                        cmdBot.CommandList(message);
                        break;

                    case string s when (s == "roll" || s == "ролл"):
                        cmdBot.Roll(message);
                        break;

                    case string s when (s == "another" || s == "другое"):
                        cmdBot.Another(message);
                        break;

                    case string s when (s == "запускаем" || s == "goose" || s == "гусь"):
                        cmdBot.Goose(message);
                        break;

                    default:
                        throw new ArgumentNullException("Command not found");
                }
            }
            catch { }
        }
    }
}
