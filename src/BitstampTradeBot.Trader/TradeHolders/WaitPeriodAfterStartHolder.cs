using System;
using BitstampTradeBot.Trader.Models;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public class WaitPeriodAfterStartHolder : ITradeHolder
    {
        private readonly TimeSpan _period;
        private DateTime _startTime;

        public WaitPeriodAfterStartHolder(TimeSpan period)
        {
            _period = period;
            _startTime = DateTime.Now;
        }

        public bool Execute(TradeSession tradeSession)
        {
            return tradeSession.Timestamp <= _startTime.Add(_period);
        }
    }
}
