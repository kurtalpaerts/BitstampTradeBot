using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public abstract class TradeRuleBase
    {
        internal readonly BitstampTrader BitstampTrader;
        internal readonly TradeSettings TradeSettings;
        internal readonly TradeSession TradeSession = new TradeSession();

        private readonly IEnumerable<ITradeHolder> _tradeHolders;

        protected TradeRuleBase(BitstampTrader bitstampTrader, TradeSettings tradeSettings, params ITradeHolder[] tradeHolders)
        {
            BitstampTrader = bitstampTrader;
            TradeSettings = tradeSettings;
            TradeSession.PairCode = tradeSettings.PairCode;
            _tradeHolders = tradeHolders;
        }

        internal abstract Task ExecuteAsync();

        protected bool ExecuteTradeHolders()
        {
            if (_tradeHolders == null) return true;

            TradeSession.Timestamp = DateTime.Now;
            TradeSession.OpenOrders = BitstampTrader.OpenOrders;

            // execute all holders and return true if 1 holder returns true
            return _tradeHolders.Select(tradeHolder => tradeHolder.Execute(TradeSession)).Any(result => result);
        }
    }
}
