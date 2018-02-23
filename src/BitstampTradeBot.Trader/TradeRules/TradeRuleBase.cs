using System.Collections.Generic;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.Helpers;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public abstract class TradeRuleBase
    {
        internal readonly BitstampTrader BitstampTrader;
        internal readonly TradeSettings TradeSettings;
        private readonly IEnumerable<ITradeHolder> _tradeHolders;

        protected TradeRuleBase(BitstampTrader bitstampTrader, TradeSettings tradeSettings, params ITradeHolder[] tradeHolders)
        {
            BitstampTrader = bitstampTrader;
            TradeSettings = tradeSettings;
            _tradeHolders = tradeHolders;
        }

        internal abstract Task ExecuteAsync();

        protected async Task<bool> ExecuteTradeHoldersAsync()
        {
            if (_tradeHolders == null) return true;

            foreach (var tradeHolder in _tradeHolders)
            {
                var result = await tradeHolder.ExecuteAsync(this);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
