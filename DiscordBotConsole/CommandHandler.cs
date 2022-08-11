using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Commands;

namespace DiscordBot
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            _commands = new CommandService();//command;
        }

        public async Task InstallCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                        services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
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
                }
            }
            catch { }

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }
    }
}
