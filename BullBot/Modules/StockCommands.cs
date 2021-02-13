using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using BullBot.Utilities;
using System.Linq;

namespace BullBot.Modules
{
    public class StockCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Stocks _stocks;

        public StockCommands(Stocks stocks)
        {
            _stocks = stocks;
        }

        [Command("symbols")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GetSymbols(string value = null)
        {
            if (value == null)
            {
                var symb = await _stocks.GetTickers();
                string replyTicker = "";
                foreach (Ticker symbol in symb)
                {
                    replyTicker += $"{symbol.Symbol}, ";
                }

                await ReplyAsync(replyTicker);
            }
            else
            {
                string msg = await _stocks.ModifyTickers(value);
                await ReplyAsync(msg);
            }
        }
    }
}
