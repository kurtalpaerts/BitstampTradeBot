using System;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.TradeRules;

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

        public Task<bool> ExecuteAsync(TradeRuleBase tradeRule)
        {
            return Task.Run(()=> DateTime.Now <= _startTime.Add(_period));
        }
    }
}
