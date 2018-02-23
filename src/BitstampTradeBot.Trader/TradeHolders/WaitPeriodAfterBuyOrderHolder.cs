using System;
using BitstampTradeBot.Trader.Models;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public class WaitPeriodAfterBuyOrderHolder : ITradeHolder
    {
        private readonly TimeSpan _period;

        public WaitPeriodAfterBuyOrderHolder(TimeSpan period)
        {
            _period = period;
        }

        public bool Execute(TradeSession tradeSession)
        {
            return tradeSession.Timestamp <= tradeSession.LastBuyTimestamp.Add(_period);
        }
    }
}
