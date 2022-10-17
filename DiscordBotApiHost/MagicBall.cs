using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotApiHost;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DiscordBot.Commands
{
    /// <summary>
    /// команды/commands - возвращает все доступные команды
    /// ролл/roll - бросает кубики в формате (кол-во)d(граней) (сложн.) или (кол-во)д(граней) (сложн.) Если не указанно - гр. 10, сл. 6
    /// wod/вод - дает ссылку на "Все Оттенки Мира Тьмы"
    /// lvlup/levelup/лвлап/левелап - отправляет картинку по прокачке
    /// aura/auras/аура/ауры - отправляет картинку по цветам в аурах
    /// вопрос - помогает принять решения (варианты ответа через запятую)
    /// another/другое - руинка
    /// </summary>
    public class CmdBot
    {
        private readonly IOptions<CmdBotConf> _cmdBotConf;
        public CmdBot(IOptions<CmdBotConf> cmdBotConf)
        {
            _cmdBotConf = cmdBotConf;
        }

        public async void CommandList(SocketUserMessage msg) =>
            await msg.Channel.SendMessageAsync(_cmdBotConf.Value.Commands);

        public async void Wod(SocketUserMessage msg) =>
            await msg.Channel.SendMessageAsync(_cmdBotConf.Value.Wod);

        public async void Another(SocketUserMessage msg)
        {
            try { await msg.Channel.SendMessageAsync($"{msg.Author.Mention}: {_cmdBotConf.Value.Another}"); }
            catch { }
        }

        public async void Roll(SocketUserMessage msg)
        {
            Random random = new Random();

            string a, b = "";

            List<int> res;

            char fnd = 'd';
            int ind, N, M, S;

            M = 10;
            S = M / 2 + 1;

            string str;

            if ((msg.Content == null) || (msg.Content.Length == 0)) return;

            try
            {
                str = msg.Content.Substring(msg.Content.IndexOf(' ')).Trim().ToLower();
                ind = str.IndexOf(fnd);
                if (ind == -1)
                {
                    fnd = 'д';
                    ind = str.IndexOf(fnd);
                }

                if (ind == -1)
                    N = Convert.ToInt32(str);
                else
                {
                    N = Convert.ToInt32(str.Substring(0, ind));
                    str = str.Substring(ind + 1);
                    if (str.IndexOf(' ') == -1)
                    {
                        M = Convert.ToInt32(str);
                        S = M / 2 + 1;
                    }
                    else
                    {
                        M = Convert.ToInt32(str.Substring(0, str.IndexOf(' ')));
                        S = Convert.ToInt32(str.Substring(str.IndexOf(' ') + 1));
                    }
                }
            }
            catch
            {
                await msg.Channel.SendMessageAsync("(кол-во)d(граней) (сложн.) или (кол-во)д(граней) (сложн.). Пример \"2d6 4\"");
                return;
            }

            res = new List<int>();
            for (int i = 0; i < N; i++) res.Add(random.Next(M) + 1);
            N = 0;

            res.Sort((x, y) => y.CompareTo(x));

            a = $"{msg.Author.Username}: ";
            foreach (int i in res)
            {
                b += $"{i} ";
                if (i >= S) N++;
            }
            a += $"**({N})** ||`{b}`||";

            await msg.Channel.SendMessageAsync(a);
        }

        public async void Levelup(SocketUserMessage msg) =>
            await msg.Channel.SendMessageAsync(_cmdBotConf.Value.LevelUp);

        public async void Auras(SocketUserMessage msg) =>
            await msg.Channel.SendMessageAsync(_cmdBotConf.Value.Auras);

        public async void AnswerMagicBall(SocketUserMessage msg, int argPos)
        {
            Random random = new Random();
            List<string> list = new List<string>()
            {
                "тупой хохол",
                "паршивый водолаз",
                "битард с двача",
                "фанат КВН",
                "дотерок",
                "победитель специальной олимпиады",
                "лолер",
                "КМС по боксу",
                "фанат Моргенштерна",
                "поднадусевый свинокарась",
                "жертва аборта",
                "js макака",
                "твой батя",
                "твой ФСБшник",
                "[ДАННЫЕ УДАЛЕНЫ]"
            };

            string a;
            List<string> lq = msg.Content.Substring(argPos).Split(',').ToList();

            lq.RemoveAll((l) => l == String.Empty);

            string[] questions = lq.ToArray();

            if (questions.Length == 0)
            {
                a = "Расскажи что за пики точеные тебя ждут, да запятые между ними не забудь, а то зашквареным будешь.";
                await msg.Channel.SendMessageAsync(a);
                return;
            }
            if (questions.Length == 1)
            {
                a = $"Да ты чё, {list[random.Next(list.Count)]}?!!! Ничего я тебе не скажу, даун.";
                await msg.Channel.SendMessageAsync(a);
                return;
            }

            if (questions.Length < 4)
                a = $"Так ты звал меня ради всего {questions.Length} стульев?\n";
            else
                a = $"Огого, выбор аж из {questions.Length} стульев ?\n";
            a += $"Ну думаю тут и {list[random.Next(list.Count)]}";
            a += $" понимает, что правильный стул — **{questions[random.Next(questions.Length)].Trim()}**";

            await msg.Channel.SendMessageAsync(a);
        }

        public async void Goose(SocketUserMessage msg) =>
            await msg.Channel.SendMessageAsync(_cmdBotConf.Value.Goose);
    }
}
