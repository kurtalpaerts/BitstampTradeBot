using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public abstract class TradeRuleBase
    {
        protected IEnumerable<ITradeHolder> TradeHolders;
        internal DateTime LastBuyTimestamp;

        protected TradeRuleBase(params ITradeHolder[] tradeHolders)
        {
            TradeHolders = tradeHolders;
        }

        internal abstract Task ExecuteAsync(BitstampTrader bitstampTrader);
    }
}
