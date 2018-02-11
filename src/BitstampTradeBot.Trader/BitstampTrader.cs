using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitstampTradeBot.Data.Models;
using BitstampTradeBot.Data.Repositories;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.TradeRules;

namespace BitstampTradeBot.Trader
{
    public class BitstampTrader
    {
        public event EventHandler<Exception> ErrorOccured;
        public event EventHandler<BitstampTicker> TickerRetrieved;

        private static int _counter;
        private Timer _mainTimer;
        private readonly int _startTime;
        private readonly int _dueTime;
        private readonly List<ITradeRule> _traderRules;
        private readonly BitstampExchange _bitstampExchange;
        private readonly SqlRepository<MinMaxLog> _minMaxLogRepository;
        private readonly SqlRepository<Order> _orderRepository;
        private readonly SqlRepository<CurrencyPair> _currencyPairRepository;
        
        public BitstampTrader(int startTime, int dueTime, params ITradeRule[] tradeRules)
        {
            _bitstampExchange = new BitstampExchange();
            _minMaxLogRepository = new SqlRepository<MinMaxLog>(new AppDbContext());
            _orderRepository = new SqlRepository<Order>(new AppDbContext());
            _currencyPairRepository = new SqlRepository<CurrencyPair>(new AppDbContext());

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
            await _traderRules[_counter].ExecuteAsync(this);

            _counter++;

            if (_counter >= _traderRules.Count)
            {
                _counter = 0;
            }
        }

        #region proxy methods

        internal async Task<BitstampTicker> GetTickerAsync(BitstampPairCode pairCode)
        {
            var ticker = await _bitstampExchange.GetTickerAsync(pairCode);
            TickerRetrieved?.Invoke(this, ticker);

            UpdateMinMaxLog(pairCode, ticker);

            return ticker;
        }

        #endregion

        #region helpers

        private void UpdateMinMaxLog(BitstampPairCode pairCode, BitstampTicker ticker)
        {
            // get the record of the current day
            var currentDay = DateTime.Now.Date;

            // get the pair code id
            var pairCodeId = _currencyPairRepository.First(c => c.PairCode == pairCode.ToString()).Id;

            // get minMaxLog from database
            var minMaxLog = _minMaxLogRepository.ToList().FirstOrDefault(l => l.Day == currentDay && l.CurrencyPairId == pairCodeId);

            // if the day record do not exist then add, otherwise update the min and max values if necessary
            if (minMaxLog == null)
            {
                _minMaxLogRepository.Add(new MinMaxLog { Day = currentDay, CurrencyPairId = pairCodeId, Minimum = ticker.Last, Maximum = ticker.Last });
            }
            else
            {
                if (minMaxLog.Minimum > ticker.Last) minMaxLog.Minimum = ticker.Last;
                if (minMaxLog.Maximum < ticker.Last) minMaxLog.Maximum = ticker.Last;
            }

            // save changes to database
            _minMaxLogRepository.Save();
        }

        #endregion helpers
    }
}
