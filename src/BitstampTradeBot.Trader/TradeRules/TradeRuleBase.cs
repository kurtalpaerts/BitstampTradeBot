using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.Helpers;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public abstract class TradeRuleBase
    {
        protected readonly TradeSettings TradeSettings;
        private readonly IEnumerable<ITradeHolder> _tradeHolders;
        internal DateTime LastBuyTimestamp;

        protected TradeRuleBase(TradeSettings tradeSettings, params ITradeHolder[] tradeHolders)
        {
            TradeSettings = tradeSettings;
            _tradeHolders = tradeHolders;
        }

        internal abstract Task ExecuteAsync(BitstampTrader bitstampTrader);

        internal bool ExecuteTradeHolders()
        {
            return _tradeHolders != null && _tradeHolders.Any(tradeHolder => tradeHolder.Execute(this));
        }
    }
}
