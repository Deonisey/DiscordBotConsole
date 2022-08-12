using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot;

namespace DisckordBotConsole
{
    internal class Program
    {
        static Task Main(string[] args) => new Program().MainAsync(args);

        public async Task MainAsync(string[] args)
        {
            DiscBot _bot = new DiscBot();

            await _bot.StartBot();
        }
    }
}
