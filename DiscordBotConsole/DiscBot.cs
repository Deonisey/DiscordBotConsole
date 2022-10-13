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
        private DiscordSocketClient _client;
        private CommandHandler? _commHandler;

        CancellationTokenSource _cancellationTokenSource;
        CancellationToken _ct;

        public Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        internal DiscBot()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _ct = _cancellationTokenSource.Token;

            if (_client == null)
            {
                _client = new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 1000 });
                _client.Log += Log;
                string token = File.ReadAllText("token");
                //Console.WriteLine($"Start with token: {token}");
                _client.LoginAsync(TokenType.Bot, token);
            }
        }

        public async Task StartBot()
        {
            await _client.StartAsync();
            _commHandler = new CommandHandler(_client);

            await _commHandler.InstallCommandAsync();
            while (true)
            {
                if (_ct.IsCancellationRequested)
                    _ct.ThrowIfCancellationRequested();

                await Task.Delay(200);
            }
        }

        internal async void Stop()
        {
            try
            {
                _cancellationTokenSource.Cancel();
                await _client.StopAsync();
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
