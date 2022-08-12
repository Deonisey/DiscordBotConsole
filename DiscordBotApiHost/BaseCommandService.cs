using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Commands;
using System.Reflection;

namespace DiscordBotApiHost
{
    public class BaseCommandService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BaseCommandService> _logger;

        public BaseCommandService(
            DiscordSocketClient client, 
            CommandService commands,
            IServiceProvider serviceProvider,
            ILogger<BaseCommandService> logger)
        {
            _client = client;
            _commands = commands;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModuleAsync<BaseCommandService>(_serviceProvider); //should work i dunno their DI configuration is weird
        }

        public async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
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

            try
            {
                await PrepareCommandContext(context, message, argPos);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
            

            await _commands.ExecuteAsync(context, argPos, _serviceProvider);
        }

        private async Task PrepareCommandContext(SocketCommandContext commandContext, SocketUserMessage message, int argPos)
        {
            switch (message.Content.Substring(argPos).Split()[0].ToLower())
            {
                case "вопрос":
                    CmdBot.AnswerMagicBall(message, argPos + 6);
                    break;

                case "wod":
                case "вод":
                    CmdBot.Wod(message);
                    break;

                case "lvlup":
                case "levelup":
                case "лвлап":
                case "левелап":
                    CmdBot.Levelup(message);
                    break;

                case "aura":
                case "auras":
                case "аура":
                case "ауры":
                    CmdBot.Auras(message);
                    break;

                case "commands":
                case "команды":
                    CmdBot.CommandList(message);
                    break;

                case "roll":
                case "ролл":
                    CmdBot.Roll(message);
                    break;
                
                case "another":
                case "другое":
                    CmdBot.Another(message);
                    break;

                case string s when (s == "1" || s == "2" || s == "3"):
                    //select stuff to do from above in a single line

                    default:                    
                    throw new ArgumentNullException("Command not found");
            }
        }
    }
}
