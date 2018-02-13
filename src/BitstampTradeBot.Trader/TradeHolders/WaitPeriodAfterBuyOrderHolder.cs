using System;
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

        public bool Execute(TradeRuleBase tradeRule)
        {
            return DateTime.Now <= tradeRule.LastBuyTimestamp.Add(_period);
        }
    }
}
