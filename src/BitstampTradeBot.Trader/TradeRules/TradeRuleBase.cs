using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.Helpers;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public abstract class TradeRuleBase
    {
        protected readonly TradeSettings TradeSettings;
        protected IEnumerable<ITradeHolder> TradeHolders;
        internal DateTime LastBuyTimestamp;

        protected TradeRuleBase(TradeSettings tradeSettings, params ITradeHolder[] tradeHolders)
        {
            TradeSettings = tradeSettings;
            TradeHolders = tradeHolders;
        }

        internal abstract Task ExecuteAsync(BitstampTrader bitstampTrader);
    }
}
