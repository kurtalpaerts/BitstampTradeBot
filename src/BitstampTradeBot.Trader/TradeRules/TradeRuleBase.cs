using System;
using System.Collections.Generic;
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
            _tradeHolders = tradeHolders;
        }

        internal abstract Task ExecuteAsync();

        protected bool ExecuteTradeHolders(TradeSession tradeSession)
        {
            if (_tradeHolders == null) return true;

            tradeSession.Timestamp = DateTime.Now;
            tradeSession.PairCode = TradeSettings.PairCode;
            tradeSession.OpenOrders = BitstampTrader.OpenOrders;

            foreach (var tradeHolder in _tradeHolders)
            {
                var result = tradeHolder.Execute(tradeSession);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
