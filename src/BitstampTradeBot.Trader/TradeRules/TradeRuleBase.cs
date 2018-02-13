using System.Collections.Generic;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public abstract class TradeRuleBase
    {
        protected IEnumerable<ITradeHolder> TradeHolders;

        protected TradeRuleBase(params ITradeHolder[] tradeHolders)
        {
            TradeHolders = tradeHolders;
        }
    }
}
