using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.TraderRules;

namespace BitstampTradeBot.Trader
{
    public class BitstampTrader
    {
        public event EventHandler<Exception> ErrorOccured;

        private readonly Timer _mainTimer;
        private readonly int _dueTime;
        private readonly List<ITraderRule> _traderRules;

        public BitstampTrader(int startTime, int dueTime, params ITraderRule[] traderRules)
        {
            _traderRules = traderRules.ToList();
            _mainTimer = new Timer(TimerCallback, null, startTime, Timeout.Infinite);
            _dueTime = dueTime;
        }

        private async void TimerCallback(object state)
        {
            try
            {
                await Trade();
            }
            catch (Exception e)
            {
                ErrorOccured?.Invoke(this, e);
            }

            _mainTimer.Change(_dueTime, Timeout.Infinite);
        }

        private async Task Trade()
        {
            foreach (var traderRule in _traderRules)
            {
                await traderRule.ExecuteAsync(this);
            }
        }
    }
}
