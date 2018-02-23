using System;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.TradeRules;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public class WaitPeriodAfterBuyOrderHolder : ITradeHolder
    {
        private readonly TimeSpan _period;

        public WaitPeriodAfterBuyOrderHolder(TimeSpan period)
        {
            _period = period;
        }

        public Task<bool> ExecuteAsync(TradeRuleBase tradeRule)
        {
            return Task.Run(() => DateTime.Now <= tradeRule.BitstampTrader.LastBuyTimestamp.Add(_period));
        }
    }
}
