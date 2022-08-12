using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{

    internal class DiscBot : IDisposable
    {
        private Task _botTask;
        public Task BotTask { get { return _botTask; } }

        private DiscordSocketClient _client;
        private CommandHandler _commHandler;

        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken _ct;

        public Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        internal DiscBot()
        {
            _ct = _cancellationTokenSource.Token;

            if (_client == null)
            {
                _client = new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 1000 });
                _client.Log += Log;

                //  You can assign your bot token to a string, and pass that in to connect.
                //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
                string token = File.ReadAllText("token");

                // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
                // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
                // var token = File.ReadAllText("token.txt");
                // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

                _client.LoginAsync(TokenType.Bot, token);
            }

            _botTask = StartBot();
            //_botTask.Start();
        }

        public async Task StartBot()
        {

            await _client.StartAsync();
            _commHandler = new CommandHandler(_client);

            await _commHandler.InstallCommandAsync();

            while (true)
            {
                if (_ct.IsCancellationRequested)
                    break; //_ct.ThrowIfCancellationRequested();

                await Task.Delay(200);
            }
        }

        internal async void Stop()
        {
            try
            {
                if (_ct != null)
                    _cancellationTokenSource.Cancel();
                await _botTask;
            }
            catch { }
        }



        public void Dispose()
        {
            Stop();
            _client.Dispose();
            _cancellationTokenSource.Dispose();
            //throw new NotImplementedException();
        }
    }
}
