using System.Collections.Generic;
using System.Threading.Tasks;
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

        internal abstract Task ExecuteAsync(BitstampTrader bitstampTrader);
    }
}
