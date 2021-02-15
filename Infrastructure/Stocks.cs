using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Stocks
    {
        private readonly BullContext _context;

        public Stocks(BullContext context)
        {
            _context = context;
        }

        public async Task<List<Ticker>> GetTickers()
        {
            var symbols = await _context.Tickers.ToListAsync();

            return await Task.FromResult(symbols);
        }

        public async Task<string> ModifyTickers(string symbol)
        {
            var modTicker = await _context.Tickers
                .Where(x => x.Symbol == symbol)
                .FirstOrDefaultAsync();

            if (modTicker == null)
            {
                string modTickMsg = $"Added {symbol} to database";
                _context.Add(new Ticker { Symbol = symbol });
                await _context.SaveChangesAsync();
                return modTickMsg;
            }
            else if (modTicker != null)
            {
                string modTickMsg = $"Removed {symbol} from database";
                _context.Remove(modTicker);
                await _context.SaveChangesAsync();
                return modTickMsg;
            }
            else
            {
                string modTickMsg = $"Unable to do action with {symbol} to database";
                return modTickMsg;
            }
        }

        public async Task<List<BullList>> GetBullList()
        {
            var BullList = await _context.BullLists.ToListAsync();
            return await Task.FromResult(BullList);
        }
    }
}
