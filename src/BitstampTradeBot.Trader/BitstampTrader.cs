using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitstampTradeBot.Data.Models;
using BitstampTradeBot.Data.Repositories;
using BitstampTradeBot.Exchange;
using BitstampTradeBot.Trader.TraderRules;

namespace BitstampTradeBot.Trader
{
    public class BitstampTrader
    {
        public event EventHandler<Exception> ErrorOccured;

        private static int _counter;
        private Timer _mainTimer;
        private readonly int _startTime;
        private readonly int _dueTime;
        private readonly List<ITradeRule> _traderRules;
        private readonly BitstampExchange _bitstampExchange = new BitstampExchange();
        private IRepository<MinMaxLog> _minMaxLogRepository;
        private IRepository<Order> _orderRepository;

        public BitstampTrader(int startTime, int dueTime, params ITradeRule[] tradeRules)
        {
            _startTime = startTime;
            _dueTime = dueTime;
            _traderRules = tradeRules.ToList();
        }

        public void Start()
        {
            _mainTimer = new Timer(TimerCallback, null, _startTime, Timeout.Infinite);
        }

        public void Stop()
        {
            _mainTimer.Dispose();
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
            await _traderRules[_counter].ExecuteAsync(_bitstampExchange, _minMaxLogRepository, _orderRepository);

            _counter++;

            if (_counter >= _traderRules.Count)
            {
                _counter = 0;
            }
        }
    }
}
